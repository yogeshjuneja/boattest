 --Store Procedure

Create procedure prcBoat
(
	@ID	bigint=NULL,
	@BoatName	nvarchar(100)=NULL,
	@HourlyRate	bigint=NULL,
	@CustomerName	bigint=NULL,
	@OutputValue BIGINT=NULL OUTPUT
)	
As
Begin
	IF(@Sptype = 1)	---- Inserting New Record
	BEGIN
		IF NOT EXISTS (select TOP 1 1 from BoatDetails where BoatName=@BoatName)
		BEGIN
			insert into BoatDetails
				(BoatName,HourlyRate)
			values
				(@BoatName,@HourlyRate)
				SET @OutputValue=IDENT_CURRENT('BoatDetails')
		END
		ELSE
			SET @OutputValue=-100
		
	END
	ELSE IF( @Sptype = 2)	---- Updating Existing Record as per Category ID
BEGIN

   IF   EXISTS (select TOP 1 1 from BoatDetails where ID=@BoatID)
   BEGIN
		IF NOT EXISTS (select TOP 1 1 from BoatRental where BoatID=@BoatID)
		BEGIN
			insert into BoatRental
				(BoatID,CustomerName)
			values
				(@ID,@CustomerName)
				SET @OutputValue=200
			END
		ELSE
			SET @OutputValue=100
	END
	ELSE 
	SET @OutputValue=404
END






End