using Microsoft.AspNetCore.Http;

public class CookieService : ICookieService
{
    public string GetCookie(HttpRequest request, string key)
    {
        return request.Cookies[key];
    }

    public void SetCookie(HttpResponse response, string key, string value, int days = 1)
    {
        CookieOptions options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(days),
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict
        };
        response.Cookies.Append(key, value, options);
    }

    public void Delete(HttpResponse response, string key, string path = "/")
    {
        response.Cookies.Delete(key, new CookieOptions { Path = path });
    }

    public void DeleteAllCookies(HttpRequest request, HttpResponse response)
    {
        foreach (string cookie in request.Cookies.Keys)
        {
            response.Cookies.Delete(cookie, new CookieOptions
            {
                Path = "/"
            });
        }
    }
}