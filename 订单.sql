create table orders(
	oid numeric(15) primary key,
	otime datetime not null,
	paid numeric(4,2) not null,
	change numeric(4,2) not null,
	total numeric(4,2) not null,
	operator numeric(4)
)
	