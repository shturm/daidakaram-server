mysql _stck -e "select concat(c.type, ' ', c.group, ' ', c.item) as Name, c.sku as SKU, c.oem as OEM, c.price as Price from _stck.sample_1 c" -u root -p > Product.csv
