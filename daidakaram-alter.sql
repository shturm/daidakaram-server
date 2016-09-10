SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

ALTER TABLE `daidakaram`.`Image` 
CHANGE COLUMN `IsThumbnail` `IsThumbnail` VARCHAR(3) NULL DEFAULT NULL COMMENT ' /* comment truncated */ /*nhibernate can discriminate on string only*/' ;

ALTER TABLE `daidakaram`.`Category` 
ADD INDEX `fk_Category_Category1_idx` (`Id` ASC, `ParentId` ASC);

ALTER TABLE `daidakaram`.`Product` 
ADD CONSTRAINT `fk_Product_ProductCar1`
  FOREIGN KEY (`Id`)
  REFERENCES `daidakaram`.`ProductCar` (`ProductId`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_Product_Image1`
  FOREIGN KEY (`Id`)
  REFERENCES `daidakaram`.`Image` (`ProductId`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `daidakaram`.`Category` 
ADD CONSTRAINT `fk_Category_Product1`
  FOREIGN KEY (`Id`)
  REFERENCES `daidakaram`.`Product` (`CategoryId`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_Category_Category1`
  FOREIGN KEY (`Id` , `ParentId`)
  REFERENCES `daidakaram`.`Category` (`ParentId` , `Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `daidakaram`.`Car` 
ADD CONSTRAINT `fk_Car_ProductCar1`
  FOREIGN KEY (`Id`)
  REFERENCES `daidakaram`.`ProductCar` (`CarId`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
