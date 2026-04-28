using LiqPaySample.Enums;

namespace LiqPaySample.Services.Payment;

public static class PaymentStateMachine
{
    private static readonly Dictionary<PaymentStatus, HashSet<PaymentStatus>> ValidTransitions = new()
    {
        [PaymentStatus.Created]    = [PaymentStatus.Pending],
        [PaymentStatus.Pending]    = [PaymentStatus.Authorized, PaymentStatus.Failed],
        [PaymentStatus.Authorized] = [PaymentStatus.Captured, PaymentStatus.Cancelled],
        [PaymentStatus.Captured]   = [PaymentStatus.Settled],
        [PaymentStatus.Settled]    = [PaymentStatus.Refunded, PaymentStatus.PartiallyRefunded],
        [PaymentStatus.PartiallyRefunded] = [PaymentStatus.Refunded],
    };

    public static bool CanTransition(PaymentStatus from, PaymentStatus to)
        => ValidTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static void EnsureTransition(PaymentStatus from, PaymentStatus to)
    {
        if (!CanTransition(from, to))
            throw new InvalidOperationException(
                $"Invalid payment state transition: {from} → {to}");
    }
}
