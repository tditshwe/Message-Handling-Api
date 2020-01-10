/*CREATE TABLE Account(
	username varchar(20) not null PRIMARY KEY,
	name varchar(50) not null,
	password varchar(200) not null,
	status varchar(100) not null,
	role varchar(12) not null,
	imageurl varchar(100),
);

CREATE TABLE Groups(
	id int not null IDENTITY(1,1) PRIMARY KEY,
	name varchar(25) not null,
	creatorusername varchar(20) not null foreign key references Account(username)
);

CREATE TABLE Message(
	id int not null IDENTITY(1,1) PRIMARY KEY,
	text varchar(max) not null,
	datesent datetime not null,
	senderusername varchar(20) not null foreign key references Account(username)
);*/

Create TABLE AccountMessage
(
	accountusername varchar(20) not null foreign key references Account(username),
	messageid int not null foreign key references Message(id)
)

/*Create TABLE AccountGroups
(
	accountusername varchar(20) not null foreign key references Account(username),
	groupsid int not null foreign key references Groups(id)
);

ALTER TABLE Account
Add name varchar(100) not null default 'User 1';

CREATE TABLE Chat(
	id int not null IDENTITY(1,1) PRIMARY KEY,
	lastmessageid int not null default 0,
	senderusername varchar(20) not null foreign key references Account(username),
	receiverusername varchar(20) not null foreign key references Account(username)*/
);