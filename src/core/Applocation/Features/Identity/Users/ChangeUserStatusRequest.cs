namespace Applocation.Features.Identity.Users;

public class ChangeUserStatusRequest
{
    public string UserId { get; set; }
    public bool Activation { get; set; }

    // 可以這樣寫解構,
    // 就可以這樣使用var(userId , activation) = request.ChangeUserStatus; userId,和activation可以改成其他名稱
    //public void Deconstruct(out string userId, out bool activation)
    //{
    //    userId = UserId;
    //    activation = Activation;
    //}
}