mysqlimport --delete --ignore-lines=1 --fields-terminated-by='\t' --lines-terminated-by='\n' --local --columns=Name,SKU,OEM,Price -u root -p daidakaram Product.csv
