create table memberlist(
	mno numeric(11) primary key,
	mname char(10) not null,
	mclass smallint constraint cclass check(mclass in(1,2,3)),
	mpoints int
)