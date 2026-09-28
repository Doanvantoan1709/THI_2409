HƯỚNG DẪN CẤU HÌNH DATABASE, CHẠY ỨNG DỤNG VÀ GỌI CÁC API MỚI

b1: Vào appsetting.json cấu hình connectionString như sau:
```
  "ConnectionStrings": {
    "DETHI_2409": "Host=10.8.0.1;Port=5432;Database=DT_2409;Username=postgres;Password=arPNmdJER6m42346;"
  }
```

b2: Mở App OpenVPN Connect, nhập tài khoản và mật khẩu để truy cập tới server chứa db

b3: Quay trở về Visual Studio và chạy chương trình "https"

b4: Truy cập Postman và nhập các endpoint sau đây:

LƯU Ý: thay đổi port localhost phù hợp

// Thi 24/09
GET: https://localhost:7107/api/health

GET: https://localhost:7107/api/work-items?overdue=true

GET: https://localhost:7107/api/work-items/{id}

DELETE: https://localhost:7107/api/work-items/{id}


// Thi 28/09
GET: https://localhost:7107/api/work-items/{id}/history?from=2026-09-04&to=2026-09-05

POST: https://localhost:7107/api/work-items/{id}/notes
body raw demo:
{
    "note": "Đã thống nhất cách xử lý với nhóm giao diện"
}






