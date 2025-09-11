using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akna_api.Domain.Entities.Base
{
    /// <summary>
    /// Tüm veri model sınıfları için temel özellikleri sağlayan soyut bir sınıf.
    /// Bu sınıf, her entity'de bulunması gereken ortak alanları (ID, oluşturulma/güncellenme/silinme tarihleri ve bu işlemleri yapan kullanıcı ID'leri) tanımlar.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Entity'nin benzersiz tanımlayıcısı (Primary Key).
        /// Genellikle veritabanında otomatik artan bir değerdir.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Entity'nin oluşturulduğu tarih ve saat.
        /// Kayıt eklendiğinde otomatik olarak ayarlanmalıdır.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Bu kaydı oluşturan kullanıcının benzersiz kimliği.
        /// Genellikle UserId tablosundan bir referanstır.
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// Entity'nin son güncellendiği tarih ve saat.
        /// Kayıt her güncellendiğinde otomatik olarak ayarlanmalıdır.
        /// Nullable olabilir, çünkü ilk oluşturulduğunda henüz güncellenmemiş olabilir.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Bu kaydı en son güncelleyen kullanıcının benzersiz kimliği.
        /// Nullable olabilir, çünkü ilk oluşturulduğunda henüz güncellenmemiş olabilir.
        /// </summary>
        public int? UpdatedByUserId { get; set; }

        /// <summary>
        /// Entity'nin mantıksal olarak silinip silinmediğini gösteren bayrak.
        /// True ise, entity silinmiş olarak kabul edilir ve genellikle aktif listelerde gösterilmez.
        /// Fiziksel silme yerine yumuşak silme (soft delete) yaklaşımını destekler.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Entity'nin silindiği tarih ve saat.
        /// Yalnızca IsDeleted true olduğunda değer alır.
        /// Nullable olabilir.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Bu kaydı silen kullanıcının benzersiz kimliği.
        /// Yalnızca IsDeleted true olduğunda değer alır.
        /// Nullable olabilir.
        /// </summary>
        public int? DeletedByUserId { get; set; }
    }
}
