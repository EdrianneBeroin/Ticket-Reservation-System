
select max(id) as id from movie;


insert into movie (mov_name,mov_dur,mov_int,dtrelease,poster,price) 
values ( 'spiderman no way home','115','25', '1/23/2022 10:28:13 PM', 'System.Byte[]', 115);

insert into movie(mov_name, mov_dur, mov_int, dtrelease, price, poster ) 
values('shang chi','115','50', '1/23/2022 10:38:38 PM',500, 'System.Byte[]')


select max(id) from movie;


delete  from movie;

delete from Movietime;

select DISTINCT STATUS,custname  from reservemovie;


select * from reservemovie where movieid ='5' and timeschedule ='12:05:00' and datereserved ='1/27/2022' ;
delete from reservemovie where id> 5;

select * from reservemovie where seats = 'a1' and timeschedule ='12:05:00' and datereserved ='1/27/2022' ;

update Movie set mov_name= 'negosyorealtalk', mov_dur = '115',  mov_int= '50',dtrelease= '2/4/2022',price= 900 where movieid='8';
update reservemovie set status = 'USED' where datereserved<'1/30/2022' and status ='RESERVED' and timeschedule < '9';

select * from reservemovie where datereserved<'1/30/2022'  and timeschedule < '12:58';
select * from movie;
--select * from reservemovie; where  custname='edrianne' and status = 'CANCELLED' and timeschedule  like '%%' and datereserved ='1/27/2022';
select top 2 * from movie order by dtrelease desc ;
insert into movie(mov_name, mov_dur, mov_int, dtrelease, price, poster ) values('snwh','115','20', '2/3/2022 5:14:41 PM',500,  /*+ img2/*ConvertImageToBytes(moviepicturebox.Image) +*/ @image)