Ticket reservation System

First tab
*Movie registration
*Display all on going movies in a datagrid
*Update movie data

Second tab
*Seats registration 
*Cancellation of reservation
*Recipt printing
*Seats availability
*Excel report

Third tab
*Showing poster on the 3 latest movie
*based on date registered
*only 3 movies will show

database
CREATE TABLE [dbo].[Movie] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [mov_name]  VARCHAR (50) NULL,
    [mov_dur]   VARCHAR (50) NULL,
    [mov_int]   VARCHAR (50) NULL,
    [dtrelease] DATE         NULL,
    [poster]    IMAGE        NULL,
    [price]     INT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [dbo].[Reservemovie] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [seats]        VARCHAR (50) NOT NULL,
    [datereserved] DATE         NOT NULL,
    [movieid]      INT          NOT NULL,
    [custname]     VARCHAR (50) NOT NULL,
    [status]       VARCHAR (50) NULL,
    [timeschedule] TIME (7)     NULL
);

CREATE TABLE [dbo].[Movietime] (
    [movieid]  INT      NOT NULL,
    [timeslot] TIME (7) NULL
);