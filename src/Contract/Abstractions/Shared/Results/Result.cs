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
            message = "Action create success",
            isSuccess = true
        };

        public static Success Update() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Action update success",
            isSuccess = true
        };

        public static Success Delete() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Action delete success",
            isSuccess = true
        };

        public static Success Logout() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Action logout success",
            isSuccess = true
        };

        public static Success RequestForgetPassword() => new Success()
        {
            status = (int)HttpStatusCode.OK,
            message = "Action logout success",
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
            message = "Action login success",
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

