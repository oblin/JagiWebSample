# 主要目標
針對共用元件，提供git服務與測試WEB平台

# 預計產生的元件
## Jagi.Core
Jagi: Primary DLL，主要目標為提供標準、可共用元件，讓未來所有專案可以使用。此方案已 MVC 為主，提供以下服務：
	• Crypto Service: 提供加、解密的元件服務
	• Repository Service: 提供標準的資料庫定義服務
	• Helpers: 提供 Extension methods 簡化操作面

## Jagi.Cache
Cache Service DLL: 結合指定的資料庫（可傳入或者使用 IoC 設定連結字串），提供代碼等相關的服務。這裡主要的目的為將資料由資料庫取出後，使用 cache 方式放在記憶體讓其他服務使用

# Jagi.Database
主要處理 TableSchema, CodeFile & CodeDetail 相關的資料，並提供 ColumnCache & CodeCache；另外也加入跟 ColumnCache 有關的 Angular HtmlTag 產生方案