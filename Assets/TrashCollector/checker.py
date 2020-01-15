import os
import sys
import random
import subprocess
from subprocess import STDOUT, check_output

logger = []

class Vector2:
    def __init__(self, x, y):
        self.x = x
        self.y = y
    
    def __eq__(self, value):
        return self.x == value.x and self.y == value.y

    def __repr__(self):
        return f'({self.x}, {self.y})'

class Player:
    def __init__(self, id):
        self.id = id
        self.x = 0
        self.y = 0
        self.point = 0
        self.alive = True
        self.shield = False

    def set_pos(self, _x, _y):
        self.x = _x
        self.y = _y

    def go(self, _x, _y):
        if self.alive and abs(self.x - _x) + abs(self.y - _y) == 1:
            logger.append(f'[{self.id}] Move from ({self.x}, {self.y}) to ({_x}, {_y})')
            self.x = _x
            self.y = _y

    def earn_point(self, v):
        if self.alive:
            logger.append(f'[{self.id}] Get {v} coins.')
            self.point += v

    def equip_shield(self):
        if self.alive:
            logger.append(f'[{self.id}] Equip shield.')
            self.shield |= 1
    
    def encounter_trap(self):
        if self.alive:
            logger.append(f'[{self.id}] Encounter trap.')
            if not self.shield:
                self.die()

    def die(self):
        if self.alive:
            logger.append(f'[{self.id}] dies.')
            self.alive = False
        
root = sys.argv[1]
player_sub = 'Players'
match_sub = 'Match'
map_sub = 'Maps'

map_id = sys.argv[2]

f = open(f'{root}/{map_sub}/{map_id}.txt')
num_rows, num_cols, num_moves = map(int, f.readline().strip().split())
board = [x.strip().split(' ') for x in f.readlines()]

players_id = sys.argv[3:5]
players = [ Player(players_id[i]) for i in range(2) ]
outputs_stream = open(f'{root}/{match_sub}/{players_id[0]}_{players_id[1]}_{map_id}.txt', 'w')

def subprocess_args(include_stdout=True):
    if hasattr(subprocess, 'STARTUPINFO'):
        si = subprocess.STARTUPINFO()
        si.dwFlags |= subprocess.STARTF_USESHOWWINDOW
        env = os.environ
    else:
        si = None
        env = None
    if include_stdout:
        ret = {'stdout': subprocess.PIPE}
    else:
        ret = {}

    ret.update({'stdin': subprocess.PIPE,
                'stderr': subprocess.PIPE,
                'startupinfo': si,
                'env': env })
    return ret

import subprocess, threading

def get_next_move(t):
    moves = [ Vector2(0, 0) for _ in range(2) ]
    for i in range(2):
        with open(f'{root}/{player_sub}/{players_id[i]}/MAP.INP', 'w') as f:
            f.write(f'{num_rows} {num_cols} {num_moves - t}\n')
            f.write(f'{players[i].x} {players[i].y} {players[1-i].x} {players[1-i].y}\n')
            f.write(f'{players[i].point} {1 if players[i].shield else 0}\n')
            f.write('\n'.join([' '.join(x) for x in board]))
        
        try:
            check_output([f'{players_id[i]}.exe'], timeout=1, cwd=f'{root}/{player_sub}/{players_id[i]}', shell=True, **subprocess_args(False))
        except Exception as e:
            logger.append(str(e))

        with open(f'{root}/{player_sub}/{players_id[i]}/MOVE.OUT', 'r') as f:
            try:
                moves[i] = Vector2(*map(int, f.readline().split()))
            except Exception as e:
                logger.append(str(e))
                moves[i] = Vector2(0, 0)
    return moves
    
def check_valid(v):
    return 1 <= v.x <= num_rows and 1 <= v.y <= num_cols and board[v.x - 1][v.y - 1] == '0'

def get_random_vec(v):
    x = random.randint(1, num_rows)
    y = random.randint(1, num_cols)
    while not check_valid(Vector2(x, y)) or Vector2(x, y) == v:
        x = random.randint(1, num_rows)
        y = random.randint(1, num_cols)
    return x, y

moves = get_next_move(0)

for i in range(2):
    if not check_valid(moves[i]):
        logger.append(f'[{players_id[i]}] Invalid starting point {moves[i]}.')
        moves[i] = Vector2(*get_random_vec(moves[1-i]))

while moves[0] == moves[1]:
    logger.append(f'[{players_id[0]}][{players_id[1]}] Coincided starting point {moves[0]}.')
    moves[0] = Vector2(*get_random_vec(Vector2(0, 0)))
    moves[1] = Vector2(*get_random_vec(moves[0]))

for i in range(2):
    logger.append(f'[{players_id[i]}] Starting point {moves[i]}.')
    players[i].set_pos(moves[i].x, moves[i].y)
outputs_stream.write(f'{players[0].x} {players[0].y} {players[1].x} {players[1].y}\n')

for move_count in range(1, num_moves):
    moves = get_next_move(move_count)

    prev_pos = [ Vector2(p.x, p.y) for p in players ]
    for i in range(2):
        players[i].go(moves[i].x, moves[i].y)
    outputs_stream.write(f'{players[0].x} {players[0].y} {players[1].x} {players[1].y}\n')
    
    if Vector2(players[0].x, players[0].y) == Vector2(players[1].x, players[1].y) \
        or (Vector2(players[0].x, players[0].y) == prev_pos[1] \
            and Vector2(players[1].x, players[1].y) == prev_pos[0]):
        players[0].die()
        players[1].die()

    for i in range(2):
        if board[players[i].x - 1][players[i].y - 1] == 'W':
            players[i].encounter_trap()
            board[players[i].x - 1][players[i].y - 1] = '0'
        elif board[players[i].x - 1][players[i].y - 1] == 'M':
            players[i].equip_shield()
            board[players[i].x - 1][players[i].y - 1] = '0'
        elif board[players[i].x - 1][players[i].y - 1] != '0':
            players[i].earn_point(int(board[players[i].x - 1][players[i].y - 1]))
            board[players[i].x - 1][players[i].y - 1] = '0'

logger.append('Final score:')
for i in range(2):
    logger.append(f'[{players_id[i]}] is {"alive" if players[i].alive else "dead"}{" and shielded" if players[i].shield else ""} with {players[i].point} coins.')

outputs_stream.writelines('\n'.join(logger))
outputs_stream.close()
