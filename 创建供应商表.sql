create table providerlist(
	pname char(9) primary key,
	ptel numeric(11) unique,
	paddress char(50) unique,
	pcontactorname char(9)
)