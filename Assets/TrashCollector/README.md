# Tổ chức thư mục

```
	<thư mục gốc>
	| - Maps
		| - <tên map>.txt
	| - Players
		| - Team01
		| - Team<id>
		| - ...
	| - Match
```

# Cách chạy

Sử dụng Python 3, chạy lệnh

```
	python checker.py <đường dẫn đến thư mục gốc> <tên map (không có .txt)> <tên đội 1> <tên đội 2>
```

Ví dụ:
```
	python checker.py Tournament maze TeamHy TeamVoDich
```

Một tập tin log sẽ được tạo ra trong ```<thư mục gốc>/Match``` dưới dạng ```<Team1>_<Team2>_<map>.txt```