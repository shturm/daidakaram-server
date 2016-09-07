SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE SCHEMA IF NOT EXISTS `daidakaram` DEFAULT CHARACTER SET utf8 ;
USE `daidakaram` ;

-- -----------------------------------------------------
-- Table `Image`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Image` ;

CREATE TABLE IF NOT EXISTS `Image` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Bytes` LONGBLOB NULL DEFAULT NULL,
  `ProductId` INT(11) NULL DEFAULT NULL,
  `IsThumbnail` TINYINT(1) NULL,
  `Created` DATETIME NULL DEFAULT NULL,
  `Updated` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `Product`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Product` ;

CREATE TABLE IF NOT EXISTS `Product` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Description` TEXT NULL DEFAULT NULL,
  `Name` VARCHAR(255) NULL DEFAULT NULL,
  `CategoryId` INT NULL,
  `Created` DATETIME NULL DEFAULT NULL,
  `Updated` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `Roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Roles` ;

CREATE TABLE IF NOT EXISTS `Roles` (
  `Id` VARCHAR(128) NOT NULL,
  `Name` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `Users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Users` ;

CREATE TABLE IF NOT EXISTS `Users` (
  `Id` VARCHAR(128) NOT NULL,
  `Email` VARCHAR(256) NULL DEFAULT NULL,
  `UserName` VARCHAR(256) NOT NULL,
  `EmailConfirmed` TINYINT(1) NOT NULL,
  `PasswordHash` LONGTEXT NULL DEFAULT NULL,
  `SecurityStamp` LONGTEXT NULL DEFAULT NULL,
  `PhoneNumber` LONGTEXT NULL DEFAULT NULL,
  `PhoneNumberConfirmed` TINYINT(1) NOT NULL,
  `TwoFactorEnabled` TINYINT(1) NOT NULL,
  `LockoutEndDateUtc` DATETIME NULL DEFAULT NULL,
  `LockoutEnabled` TINYINT(1) NOT NULL,
  `AccessFailedCount` INT(11) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `UserClaims`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `UserClaims` ;

CREATE TABLE IF NOT EXISTS `UserClaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `UserId` VARCHAR(128) NOT NULL,
  `ClaimType` LONGTEXT NULL DEFAULT NULL,
  `ClaimValue` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Id` (`Id` ASC),
  INDEX `UserId` (`UserId` ASC),
  CONSTRAINT `ApplicationUser_Claims`
    FOREIGN KEY (`UserId`)
    REFERENCES `Users` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `UserLogins`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `UserLogins` ;

CREATE TABLE IF NOT EXISTS `UserLogins` (
  `LoginProvider` VARCHAR(128) NOT NULL,
  `ProviderKey` VARCHAR(128) NOT NULL,
  `UserId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`, `UserId`),
  INDEX `ApplicationUser_Logins` (`UserId` ASC),
  CONSTRAINT `ApplicationUser_Logins`
    FOREIGN KEY (`UserId`)
    REFERENCES `Users` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `UserRoles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `UserRoles` ;

CREATE TABLE IF NOT EXISTS `UserRoles` (
  `UserId` VARCHAR(128) NOT NULL,
  `RoleId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  INDEX `IdentityRole_Users` (`RoleId` ASC),
  CONSTRAINT `ApplicationUser_Roles`
    FOREIGN KEY (`UserId`)
    REFERENCES `Users` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `IdentityRole_Users`
    FOREIGN KEY (`RoleId`)
    REFERENCES `Roles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `Category`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Category` ;

CREATE TABLE IF NOT EXISTS `Category` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(45) NULL,
  `ParentId` INT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Car`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Car` ;

CREATE TABLE IF NOT EXISTS `Car` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Make` VARCHAR(45) NULL,
  `Model` VARCHAR(45) NULL,
  `Variant` VARCHAR(45) NULL,
  `Body` VARCHAR(45) NULL,
  `Type` VARCHAR(45) NULL,
  `YearFrom` INT NULL,
  `YearTo` INT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ProductCar`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ProductCar` ;

CREATE TABLE IF NOT EXISTS `ProductCar` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `ProductId` INT NULL,
  `CarId` INT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
