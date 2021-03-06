﻿
CREATE PROCEDURE [dbo].[AllCommonDataTypesStoredProcedure]
    @BigInt             bigint
,   @Binary             binary(8)
,   @Bit                bit
,   @Char               char(3)
,   @Date               date
,   @DateTime           datetime
,   @DateTime2          datetime2
,   @Decimal            decimal(18,2)
,   @float              float
,   @image              image
,   @Int                int
,   @Money              money
,   @NChar              nchar(5)
,   @NText              ntext
,   @Numeric            numeric(18,2)
,   @NVarchar           nvarchar(8)
,   @Real               real
,   @Smalldatetime      smalldatetime
,   @Smallint           smallint
,   @Smallmoney         smallmoney
,   @Text               text
,   @Time               time
,   @Timestamp          timestamp
,   @Tinyint            tinyint
,   @Uniqueidentifier   uniqueidentifier
,   @Varbinary          varbinary(3)
,   @Varchar            varchar(7)
,   @Xml                xml
AS
    SELECT 
        @BigInt         [BigInt]
,       @Binary         [Binary]
,       @Bit            [Bit]
,       @Char           [Char]
,       @Date           [Date]
,       @DateTime       [DateTime]
,       @DateTime2      [DateTime2]
,       @Decimal        [Decimal]
,       @float          [Float]
,       @image          [Image]
,       @Int            [int]
,       @Money          [Money]
,       @NChar          [NChar]
,       @NText          [NText]
,       @Numeric        [Numeric]
,       @NVarchar       [NVarchar]
,       @Real           [Real]
,       @Smalldatetime  [SmallDateTime]
,       @Smallint       [SmallInt]
,       @Smallmoney     [SmallMoney]
,       @Text           [Text]
,       @Time           [Time]
,       @Timestamp      [TimeStamp]
,       @Tinyint            [TinyInt]
,       @Uniqueidentifier   [UniqueIdentifier]
,       @Varbinary          [VarBinary]
,       @Varchar            [VarChar]
,       @Xml                [Xml]
RETURN 0