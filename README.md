AdisyonWeb Proje Sunumu
.NET 9 MVC ile Restoran Adisyon Yönetim Sistemi
 AdisyonWeb isimli restoran adisyon uygulamasının kod yapısının, MVC mimarisine göre oluşturulan katmanlarının ve yazılan dosyaların açıklandığı README dosyası.
Uygulamayı ayağa kaldırmak için terminalde:

	1-dotnet restore (bağımlılıkları yükler)
	2-dotnet ef database update (veritabanı oluşur)
	3-dotnet build (build alır)
	2-dotnet run (uygulamayı başlatır)

komutları çalıştırılır.
________________________________________
 Projenin Amacı
Bu uygulamanın amacı; bir restorandaki:
•	Masaların dolu/boş durumunu takip etmek,
•	Menüdeki ürün stoklarını ve fiyatlarını yönetmek,
•	Masalara sipariş eklemek,
•	Siparişlerin adetlerini artırıp/azaltmak,
•	Ödeme almak (nakit/kart),
•	Ödeme sonrası masayı tekrar boş duruma geçirmek
gibi temel adisyon fonksiyonlarını yönetebilen bir sistem geliştirmektir.
________________________________________
Katmanlar ve Kod Yapısı
Projemiz ASP.NET Core MVC yapısını takip eder:
•	Models (Entities) → Veritabanı varlıklarımız
•	Controllers → Ekranların iş mantığını yöneten sınıflar
•	Views → Kullanıcıya görünen HTML sayfaları
•	ViewModels → View’lere özel veri taşıyan sınıflar
•	Data (DbContext) → EF Core veri erişim katmanı
Her bir parçayı detaylı biçimde açıklıyorum.
________________________________________
 Data Katmanı → AppDbContext.cs
Dosya: Data/AppDbContext.cs
Bu sınıf veritabanına bağlanmak ve tabloları EF Core ile yönetmek için kullanılır.
İçindeki kod:
•	Tabloları DbSet<> ile projeye tanıtır
•	Veritabanı bağlantısını ayarlar
•	İlişkileri (One-to-Many, Cascade vb.) tanımlar
Örnek tablolar:
public DbSet<RestaurantTable> Tables { get; set; }
public DbSet<MenuCategory> MenuCategories { get; set; }
public DbSet<MenuItem> MenuItems { get; set; }
public DbSet<Order> Orders { get; set; }
public DbSet<OrderItem> OrderItems { get; set; }
public DbSet<Payment> Payments { get; set; }
Bu dosya, uygulamanın merkezi veritabanı yöneticisi gibidir.
________________________________________
 Models (Entities) – Varlık Sınıfları
Projenin iş modelleri burada bulunur.
•	RestaurantTable.cs
Masa numarası, masa durumu (boş/dolu) gibi bilgiler içerir.
•	MenuCategory.cs
“Başlangıçlar, Ana Yemekler, Pizzalar” gibi kategorileri tutar.
•	MenuItem.cs
Her kategorideki ürünlerin adı, açıklaması, fiyatı ve stok miktarı burada tanımlanır.
•	Order.cs
Bir masaya açılan siparişin genel bilgilerini tutar (tarih, toplam tutar vb.)
•	OrderItem.cs
Bir siparişte seçilen her ürün için satır temsil eder. Miktar, birim fiyat vb.
•	Payment.cs
Ödeme şekli (nakit/kart), verilen para, para üstü gibi bilgileri içerir.
Bu varlık dosyaları, uygulamanın iş mantığını temsil eden gerçek veri modelleridir.
________________________________________
 Controllers
Her controller bir sayfanın iş mantığını yönetir.
________________________________________
a) TablesController.cs
Dosya: Controllers/TablesController.cs
Bu controller masaların sayfasını yönetir.
Görevleri:
•	Masaları listelemek
•	Masanın dolu/boş durumunu göstermek
•	Bir masaya tıklanınca sipariş ekranına yönlendirmek
Kullandığımız View:
→ Views/Tables/Index.cshtml
________________________________________
b) OrdersController.cs
Dosya: Controllers/OrdersController.cs
Bu, uygulamanın en önemli controller'ıdır.
Sipariş işlemlerinin tamamını yönetir.
Yaptıkları:
✔ Sipariş Açma
•	Eğer masada açık sipariş yoksa yeni sipariş oluşturur.
•	Masayı dolu yapar.
✔ Sipariş Ürünleri Listeleme
•	Masaya ait tüm sipariş satırlarını getirir.
✔ Siparişe Ürün Ekleme
•	Sağdaki menüden “Ekle”ye basınca ilgili ürünü siparişe ekler.
•	Stok düşürür.
✔ Adet Artırma / Azaltma / Ürün Silme
•	Sipariş satırındaki miktarı artırır veya azaltır.
•	Azalınca stok artırılır.
•	Sıfıra düşerse ürün satırı tamamen silinir.
✔ Ödeme Alma (Nakit/Kart)
•	Ödemeyi işler
•	Nakitse verilen paradan para üstü hesaplar
•	Siparişi kapatır
•	Masayı tekrar boş duruma getirir
Kullandığı View’ler:
•	Views/Orders/Manage.cshtml → Sipariş ekranı
•	Views/Orders/Payment.cshtml → Ödeme ekranı
________________________________________
c) MenuController.cs
Dosya: Controllers/MenuController.cs
Görevi:
•	Tüm menüyü kategorilere göre listelemek
•	Menü sayfasını görüntülemek
Kullandığı View:
→ Views/Menu/Index.cshtml
________________________________________
 ViewModels
ViewModel'ler ekrana özel veri taşımak için kullanılır.
________________________________________
OrderManageViewModel.cs
Bu model sipariş ekranına şunları taşır:
•	Masa bilgisi
•	Açık sipariş
•	Sipariş kalemleri
•	Menü ürünleri (kategorileriyle birlikte)
Bu ViewModel sayesinde “tek ekranda hem sipariş hem menü” gösterilebiliyor.
________________________________________
PaymentViewModel.cs
Ödeme ekranı için:
•	OrderId, TableId
•	Toplam tutar
•	Ödeme tipi (nakit/kart)
•	Verilen para
•	Para üstü
•	Hata mesajları
gibi bilgileri taşır.
________________________________________
 Views (Kullanıcı Arayüzü)
Uygulamanın ön yüzünü oluşturan HTML + Razor dosyalarıdır.
________________________________________
a) _Layout.cshtml
Bu dosya tüm sayfaların ortak iskeletidir.
İçerir:
•	Navbar (Masalar / Menü)
•	Bootstrap CSS / JS
•	Sayfa gövdesi alanı
Bütün sayfalar bu şablon üzerinden render edilir.
________________________________________
b) Masalar Ekranı
Dosya: Views/Tables/Index.cshtml
Görevleri:
•	Masaları grid halinde gösterir
•	Boş masa → Yeşil
•	Dolu masa → Kırmızı
•	Tıklanınca ilgili masanın sipariş ekranına yönlendirir
________________________________________
c) Sipariş Yönetim Ekranı
Dosya: Views/Orders/Manage.cshtml
Bu ekranda:
Sol taraf:
•	Sipariş satırları
•	Adet artır/azalt
•	Satır sil
•	Toplam fiyat
•	Ödemeye Geç butonu
Sağ taraf:
•	Kategorilere göre ayrılmış accordion menü
•	Her kategori tıklanınca aşağı kayarak açılır
•	Ürün listesi ve “Ekle” butonu
Bu ekran, gerçek bir restoran POS cihazı gibi çalışır.
________________________________________
d) Ödeme Ekranı
Dosya: Views/Orders/Payment.cshtml
İçerik:
•	Toplam tutar gösterilir
•	Ödeme tipi seçilir (nakit/kart)
•	Nakitse:
o	“Verilen para” alanı açılır
o	JS ile anlık para üstü hesaplama yapılır
•	“Ödemeyi Tamamla” butonu işlemi tamamlar
________________________________________
e) Menü Sayfası
Dosya: Views/Menu/Index.cshtml
•	Tüm kategoriler kart şeklinde görüntülenir
•	Altında o kategoriye ait tüm ürünler listelenir
•	Dünya standartlarında bir “menü görüntüleme” sayfasıdır
________________________________________
Uygulamanın Akışı
1.	Kullanıcı Masalar ekranına gelir
2.	Bir masaya tıklar
3.	Masanın sipariş yönetim ekranı açılır
4.	Sağdaki kategorilerden ürün ekler
5.	Sipariş satırları solda güncellenir
6.	Ödemeye geçilir
7.	Ödeme yapılır
8.	Masa tekrar boş duruma döner
________________________________________
 Sonuç
Bu proje, bir restoranın adisyon yönetim sürecini uçtan uca kapsayan modern bir web uygulamasıdır.
Kullandığımız teknolojiler:
•	ASP.NET Core MVC
•	.NET 9
•	Entity Framework Core
•	Bootstrap 5
•	SQL Server
•	Razor Views
•	JavaScript ile dinamik para üstü hesaplama
•	UI/UX için accordion menü tasarımı
Sistem tamamen çalışır durumda ve gerçek bir işletmede kullanılabilecek yapıdadır.
________________________________________ 

