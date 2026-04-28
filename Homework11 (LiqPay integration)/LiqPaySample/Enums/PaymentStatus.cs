namespace LiqPaySample.Enums;

public enum PaymentStatus
{
    Created,      // Щойно створений, ще не відправлений до PSP
    Pending,      // Відправлений до PSP, очікуємо відповідь
    Authorized,   // Авторизований (кошти зарезервовані)
    Captured,     // Кошти захоплені (для провайдерів з preauth)
    Settled,      // Успішно розрахований
    Failed,       // Відхилений PSP або банком-емітентом
    Cancelled,    // Скасований до авторизації (void)
    Refunded,     // Повернений (повністю)
    PartiallyRefunded // Частково повернений
}
