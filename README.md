
# 🧾 MvcOnlineTicariOtomasyon

Yunus Emre Karabayır tarafından geliştirilen bu proje, **ASP.NET MVC** ve **Microsoft SQL Server** teknolojileriyle oluşturulmuş, çok yönlü bir **Online Ticari Otomasyon Sistemi**dir.

---

## 🎯 Amaç

Bu sistem, işletmelerin ticari faaliyetlerini dijital ortama taşıyarak;

- Ürün ve stok takibi,
- Cari hesap yönetimi,
- Fatura işlemleri,
- Departman ve personel takibi,
- Admin yetkilendirmesi

gibi işlevleri merkezi ve kullanıcı dostu bir yapı altında sunar.

---

## ⚙️ Kullanılan Teknolojiler

| Katman     | Teknoloji                        |
|------------|----------------------------------|
| Backend    | ASP.NET MVC (.NET Framework)     |
| ORM        | Entity Framework (Code First)    |
| Veritabanı | Microsoft SQL Server             |
| Frontend   | HTML5, CSS3, Razor View, Bootstrap |
| Ekstra     | LINQ, Partial View, Layout Pages |

---

## 🧩 Modüller

- 🔐 **Admin Paneli**  
  Giriş, rol yönetimi, kullanıcı kontrolü

- 📦 **Ürün/Kategori Yönetimi**  
  Ürün ekleme, silme, stok takibi

- 🏢 **Departman & Personel Yönetimi**  
  Departmanlara göre personel kaydı ve görev tanımı

- 👥 **Cari İşlemleri**  
  Müşteri ve tedarikçi kayıtları, raporlama

- 🧾 **Fatura Yönetimi**  
  Fatura oluşturma, ödeme durumu takibi

---

## 🚀 Kurulum

### 1️⃣ Klonla

```bash
git clone https://github.com/ynsemr46/MvcOnlineTicariOtomasyon.git
cd MvcOnlineTicariOtomasyon
```

### 2️⃣ Visual Studio ile Aç

`MvcOnlineTicariOtomasyon.sln` dosyasını açın.

### 3️⃣ Bağlantı Dizesini Ayarla

`Web.config` dosyasındaki connection string'i kendi SQL Server ortamınıza göre güncelleyin:

```xml
<add name="DefaultConnection" 
     connectionString="Data Source=.;Initial Catalog=DbOnlineTicariOtomasyon;Integrated Security=True" 
     providerName="System.Data.SqlClient" />
```

### 4️⃣ NuGet Paketlerini Güncelle

```powershell
Update-Package -reinstall
```

### 5️⃣ Veritabanı Oluştur

Çalıştırıldığında EF Code First ile veritabanı otomatik oluşur. Gerekirse:

```powershell
Update-Database
```

---

## 📸 Ekran Görüntüleri

#### 🔐 Giriş Ekranı

![Giriş Ekranı](https://github.com/user-attachments/assets/d9d7a726-9554-4b67-891b-7a35b9053bae)

#### 📂 Kategori Yönetimi

![Kategori Yönetimi](https://github.com/user-attachments/assets/d1472947-7e9f-41e7-a1d1-eaa97076816f)

#### 🧾 Fatura Yönetimi

![Fatura Yönetimi](https://github.com/user-attachments/assets/9acfa433-d0b6-47b9-b5fe-894248dc02b3)

#### 📊 Satış Raporu & Grafik

![Satış Raporu](https://github.com/user-attachments/assets/a6a1fef1-b9b8-45e6-8d5c-7faf19dc00ca)

#### 🧑‍💼 Personel Paneli

![Personel Paneli](https://github.com/user-attachments/assets/6cf5a6f4-ead5-447b-8786-dc671b5110e7)

#### 👤 Admin Bilgileri

![Admin Bilgi Paneli](https://github.com/user-attachments/assets/2698ac93-2916-40b6-a5b4-00d42c430316)

---

## 👨‍💻 Geliştirici

- **Ad Soyad:** Yunus Emre Karabayır  
- **Öğrenci No:** 23300031335  
- **GitHub:** [github.com/ynsemr46](https://github.com/ynsemr46)

---

## 📄 Lisans

> 🔒 **Bu proje şahsıma aittir.**  
> Sadece **kişisel ve eğitim amaçlı** kullanılabilir.  
> **Herhangi bir ticari amaçla kullanılması kesinlikle yasaktır.**  
> İzinsiz çoğaltılması, satılması veya paylaşılması durumunda yasal işlem uygulanacaktır.

---
