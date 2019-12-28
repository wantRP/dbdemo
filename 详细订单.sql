create table orderdetail(
	oid numeric(15),
	ono numeric(4),
	oname char(10),
	oquantity smallint,
	oprice numeric(4,2),
	ototal numeric(4,2),
	foreign key(oid) references orders(oid),
	foreign key(ono) references goods(gno),
	foreign key(oname) references goods(gname),

)