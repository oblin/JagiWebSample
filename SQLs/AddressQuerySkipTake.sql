with orderedAddress as ( select Zip, County, Realm, Street, ROW_NUMBER() OVER (ORDER BY Zip) AS 'RowNumber' from Address )

select * from orderedAddress where RowNumber between 0 and 24;