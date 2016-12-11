SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL';

ALTER TABLE `default_schema`.`Product` 
ADD COLUMN `CompatibilityStatus` INT(11) NULL DEFAULT NULL AFTER `OEM`;

CREATE TABLE IF NOT EXISTS `default_schema`.`CompatibilitySetting` (
  `Id` VARCHAR(36) NOT NULL,
  `Make` VARCHAR(45) NULL DEFAULT NULL,
  `Model` VARCHAR(45) NULL DEFAULT NULL,
  `Variant` VARCHAR(45) NULL DEFAULT NULL,
  `ProductId` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

DROP TABLE IF EXISTS `default_schema`.`stcvols` ;

DROP TABLE IF EXISTS `default_schema`.`stctypes` ;

DROP TABLE IF EXISTS `default_schema`.`stcitems` ;

DROP TABLE IF EXISTS `default_schema`.`stcgroups` ;

DROP TABLE IF EXISTS `default_schema`.`mesutypes` ;

DROP TABLE IF EXISTS `default_schema`.`mesures` ;

DROP TABLE IF EXISTS `default_schema`.`ProductCar` ;


-- -----------------------------------------------------
-- Placeholder table for view `default_schema`.`vLegacy`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `default_schema`.`vLegacy` (`type` INT, `group` INT, `item` INT, `item_code` INT, `sku` INT, `oem` INT);


USE `default_schema`;

-- -----------------------------------------------------
-- View `default_schema`.`vLegacy`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `default_schema`.`vLegacy`;
DROP VIEW IF EXISTS `default_schema`.`vLegacy` ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
