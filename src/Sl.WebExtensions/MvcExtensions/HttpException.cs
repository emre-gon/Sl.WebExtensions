using System;
using System.Net;

namespace Sl.WebExtensions.MvcExtensions
{
    public class HttpException : Exception
    {
        public HttpException(string Message, HttpStatusCode HttpStatusCode)
        : base(Message)
        {
            this.HttpStatusCode = HttpStatusCode;
        }

        public HttpException()
            : this("Bilinmeyen bir hata oluştu.", HttpStatusCode.InternalServerError)
        {

        }

        public HttpStatusCode HttpStatusCode { get; private set; }
    }

    /// <summary>
    /// 403
    /// </summary>
    public class ForbiddenException : HttpException
    {
        public ForbiddenException(string Message)
            : base(Message, HttpStatusCode.Forbidden)
        {

        }

        public ForbiddenException()
            : this("Bu işlem için yetkiniz yok.")
        {

        }
    }

    /// <summary>
    /// 401
    /// </summary>
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException(string Message)
            : base(Message, HttpStatusCode.Unauthorized)
        {

        }

        public UnauthorizedException()
            : this("Bu işlem için giriş yapmalısınız.")
        {

        }
    }

    /// <summary>
    /// 400
    /// </summary>
    public class BadRequestException : HttpException
    {
        public BadRequestException(string Message)
            : base(Message, HttpStatusCode.BadRequest)
        {

        }
    }

    /// <summary>
    /// 404
    /// </summary>
    public class NotFoundException : HttpException
    {
        public NotFoundException(string Message)
            : base(Message, HttpStatusCode.NotFound)
        {

        }

        public NotFoundException()
        : base("Bulunamadı.", HttpStatusCode.NotFound)
        {

        }
    }
}