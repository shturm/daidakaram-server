-- products with categories and images

select 
    p.Id,
    p.SKU,
    c.Name CategoryName,
    cp.Name CategoryParent,
    i.Id ImageId
from
    daidakaram.Product p
        left join
    daidakaram.Category c ON p.CategoryId = c.Id
        left join
    daidakaram.Category cp ON c.ParentId = cp.Id
        left join
    daidakaram.Image i ON i.ProductId = p.Id;

select 
    _stck.sample_1.type,
    _stck.sample_1.group,
    _stck.sample_1.item,
    _stck.sample_1.sku,
    _stck.sample_1.price
from
    _stck.sample_1
where
    sku like '%13519%';

truncate daidakaram.Product;
truncate daidakaram.Category;


select 
    *
from
    _stck.test_2
order by _stck.test_2.user_number
limit 50 offset 0;

explain select count(price) from _stck.test_2;
select count(price) from _stck.test_2;
select count(*) from _stck.stcvols;


select 
	`st`.`name` AS `type`,
	`sg`.`name` AS `group`,
	`si`.`name` AS `item`,
	`sv`.`user` AS `sku`,
	`sv`.`man` AS `oem`
from
	`stcvols` `sv`
		left join `stctypes` `st` ON `sv`.`type` = `st`.`code`
		left join `stcgroups` `sg` ON `sg`.`code` = `sv`.`group`
		left join `stcitems` `si` ON `si`.`code` = `sv`.`code`
limit 50 offset 600;