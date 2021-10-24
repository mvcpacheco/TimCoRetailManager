CREATE PROCEDURE [dbo].[spSale_SaleReport]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT A.SaleDate, A.SubTotal, A.Tax, A.Total, B.FirstName, B.LastName, B.EmailAddress
	FROM dbo.Sale A
	INNER JOIN dbo.[User] B ON A.CashierId = B.Id
END