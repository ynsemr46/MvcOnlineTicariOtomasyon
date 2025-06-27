# 🧾 MvcOnlineTicariOtomasyon

Yunus Emre Karabayır tarafından geliştirilen bu proje, **ASP.NET MVC** teknolojisi ve **Microsoft SQL Server** veritabanı kullanılarak oluşturulmuş, çok yönlü bir **Online Ticari Otomasyon Sistemidir**.

## 🎯 Projenin Amacı

Bu sistem; küçük ve orta ölçekli işletmelerin günlük ticari faaliyetlerini **dijital ortamda yönetmesini** sağlamak amacıyla geliştirilmiştir. Proje, aşağıdaki temel işlevleri kapsar:

- Ürün ve stok takibi
- Cari hesap yönetimi (müşteri/tedarikçi)
- Fatura işlemleri
- Personel ve departman takibi
- Admin yetkilendirme sistemi

---

## ⚙️ Kullanılan Teknolojiler

| Katman        | Teknoloji                          |
|---------------|------------------------------------|
| Backend       | ASP.NET MVC (.NET Framework)       |
| Veritabanı    | Microsoft SQL Server               |
| ORM           | Entity Framework (Code First)      |
| Frontend      | HTML5, CSS3, Razor View Engine     |
| UI Framework  | Bootstrap (responsive yapı)        |
| Ekstralar     | LINQ, Partial Views, Layout Pages  |

---

## 🧩 Modüller & Özellikler

### 🔐 Admin Paneli
- Admin girişi ve rol yönetimi
- Kullanıcı yetkilendirme

### 📦 Ürün ve Kategori Yönetimi
- Ürün ekleme, düzenleme, silme
- Kategori oluşturma ve güncelleme
- Stok takibi ve ürün detayları

### 🏢 Departmanlar & Personel
- Departman tanımları
- Personel bilgileri (maaş, görev, departman)

### 👥 Cari İşlemleri
- Müşteri ve tedarikçi kayıtları
- Cari hesap ekstresi ve takibi

### 🧾 Fatura Yönetimi ve Satış
- Fatura oluşturma ve ödeme durumu takibi
- Faturaya ürün ekleme
- Satış Yapma Ve satış Takibi

---

## 🔧 Kurulum Adımları

Projenizi yerel ortamda çalıştırmak için aşağıdaki adımları izleyin:

### 1️⃣ Projeyi Klonlayın

```bash
git clone https://github.com/ynsemr46/MvcOnlineTicariOtomasyon.git
cd MvcOnlineTicariOtomasyon

2️⃣ Visual Studio ile Açın
MvcOnlineTicariOtomasyon.sln dosyasını Visual Studio 2019/2022 ile açın.

3️⃣ Veritabanı Bağlantısını Yapılandırın
Web.config dosyasındaki connectionStrings bölümünü kendi SQL Server instance’ınıza göre güncelleyin:
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Data Source=.;Initial Catalog=DbOnlineTicariOtomasyon;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
4️⃣ Paketleri Geri Yükleyin
Menüden: Tools > NuGet Package Manager > Package Manager Console'a girin ve şu komutu çalıştırın:

powershell
Kopyala
Düzenle
Update-Package -reinstall
5️⃣ Veritabanını Oluşturun
Code First yaklaşımıyla veritabanı otomatik oluşturulacaktır.

Gerekirse Update-Database komutu kullanılabilir.

6️⃣ Uygulamayı Çalıştırın
F5 ile çalıştırın, sistem varsayılan olarak http://localhost:xxxx/ üzerinden açılacaktır.

📸 Ekran Görüntüleri

![Ekran görüntüsü 2025-06-27 130641](https://github.com/user-attachments/assets/d9d7a726-9554-4b67-891b-7a35b9053bae)

![Ekran görüntüsü 2025-06-27 130704](https://github.com/user-attachments/assets/d1472947-7e9f-41e7-a1d1-eaa97076816f)


![Ekran görüntüsü 2025-06-27 130718](https://github.com/user-attachments/assets/9acfa433-d0b6-47b9-b5fe-894248dc02b3)


![Ekran görüntüsü 2025-06-27 130754](https://github.com/user-attachments/assets/a6a1fef1-b9b8-45e6-8d5c-7faf19dc00ca)


![Ekran görüntüsü 2025-06-27 130803](https://github.com/user-attachments/assets/6cf5a6f4-ead5-447b-8786-dc671b5110e7)


![Ekran görüntüsü 2025-06-27 130823](https://github.com/user-attachments/assets/2698ac93-2916-40b6-a5b4-00d42c430316)



### 👨‍💻 Geliştirici
Ad Soyad: Yunus Emre Karabayır
Github: github.com/ynsemr46

⚠️ Lisans ve Kullanım Koşulları
📌 Uyarı:
Bu proje tamamen Yunus Emre Karabayır'a aittir.
Kodlar sadece eğitim ve kişisel geliştirme amaçlı kullanılabilir.
Herhangi bir ticari amaçla kullanılması yasaktır.
Ticari kullanım, izinsiz çoğaltma veya satışı durumlarında hukuki işlem uygulanabilir.
