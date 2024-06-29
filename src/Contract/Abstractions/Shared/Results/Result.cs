using System.Net;

namespace Contract.Abstractions.Shared.Results;
public class Result
{
    public class Success : IResult
    {
        private Success() { }

        public static Success Create() => new Success()
        {
            status = (int)HttpStatusCode.Created,
            message = "Tạo thành công",
            isSuccess = true
        };

        public static Success Update() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Chỉnh sửa thành công",
            isSuccess = true
        };

        public static Success Delete() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Xóa thành công",
            isSuccess = true
        };

        public static Success Logout() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Đăng xuất thành công",
            isSuccess = true
        };

        public static Success RequestForgetPassword() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Yêu cầu đặt lại mật khẩu thành công",
            isSuccess = true
        };
    }

    public class Success<TData> : IResult
    {
        public TData data { get; set; }
        private Success() { }
        public static Success<TData> Get(TData tData) => new Success<TData>()
        {
            status = (int)HttpStatusCode.OK,
            message = "Action get success",
            data = tData,
            isSuccess = true
        };

        public static Success<TData> Login(TData tData) => new Success<TData>()
        {
            status = (int)HttpStatusCode.OK,
            message = "Đăng nhập thành công",
            data = tData,
            isSuccess = true
        };

        public static Success<TData> Upload(TData tData) => new Success<TData>()
        {
            status = (int)HttpStatusCode.Created,
            message = "Action upload file success",
            data = tData,
            isSuccess = true
        };
    }

    public class Failure : IResult
    {
    }

    public class Failure<TErrors> : IResult
    {
        public TErrors errors { get; set; }
    }
}

