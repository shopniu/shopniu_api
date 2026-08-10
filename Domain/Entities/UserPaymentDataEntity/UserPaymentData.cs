
using System.ComponentModel.DataAnnotations;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Entities.common;
using Shopniu_api.Domain.Entities.PaymentDetailsEntity;

namespace Shopniu_api.Domain.Entities.UserPaymentDataEntity;

public class UserPaymentData : BaseEntity
{
    public string? CardNumber { get; set; }
    [Required]
    public string CardHolderName { get; set; }
    [Required]
    public string Address { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    [Required]
    public int LastFour { get; set; }
    public int UserId { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }


    public UserPaymentData(string cardNumber, string cardHolderName, string address, string phoneNumber, int lastFour, int userId, PaymentMethodType paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new ValidationsException("Card holder name cannot be empty.");
        if (string.IsNullOrWhiteSpace(address))
            throw new ValidationsException("Address cannot be empty.");
        if (lastFour < 0 || lastFour > 9999)
            throw new ValidationsException("Last four digits must be a positive integer between 0 and 9999.");
        if (userId <= 0)
            throw new ValidationsException("User ID must be a positive integer.");

        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        Address = address;
        PhoneNumber = phoneNumber;
        LastFour = lastFour;
        UserId = userId;
        PaymentMethod = paymentMethod;
    }
}