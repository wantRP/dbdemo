create table goods(
	standardid numeric(11)primary key,
	gno numeric(4) unique,
	unit char(3) unique,
	price numeric(4,2),
	name char(10) unique
)