# Báo cáo sau Phần 1

Nộp báo cáo sau ba ngày kể từ buổi làm Phần 1. Hãy viết ngắn gọn, bằng cách hiểu của bạn và dựa trên những việc bạn thực sự đã làm.

- Em đã xem kỹ lại những phần chưa làm được và đã hoàn thiện và bổ sung những API sau:
// Bổ sung đầy đủ 27/09
GET: https://localhost:7107/api/reports/project-summary?minItems=3

POST: https://localhost:7107/api/work-items
body raw demo:
{
  "title": "demo create",
  "description": "demo create",
  "projectCode": "WEB", 
  "assigneeId": 1,
  "priority": "High",
  "dueAt": "2026-10-01T17:00:00Z",
  "labels": [
    "backend",
    "Backend",
    "Login"
  ]
}

PATCH: https://localhost:7107/api/work-items/{id}/assignee
body raw demo:
{ "assigneeId": 2, "note": "Chuyển cho font end" }

## 1. Kết quả ở Phần 1

Bạn đã hoàn thành phần nào? Phần nào còn dở? Hãy nêu một vấn đề đã khiến bạn mất nhiều thời gian và cách bạn xử lý lúc đó.

- Ở kết quả phần 1 em chỉ làm được 4 API nhưng chưa đúng hoàn toàn như sau:
GET: https://localhost:7107/api/health

GET: https://localhost:7107/api/work-items?overdue=true

DELETE: https://localhost:7107/api/work-items/{id}

- và 1 API chưa xong: 

GET: https://localhost:7107/api/work-items/{id}

. Lý do là đọc không rõ đề và không đủ thời gian

## 2. Những gì đã học thêm

Chọn 2 hoặc 3 vấn đề trong bài làm. Với mỗi vấn đề, ghi nguyên nhân bạn tìm được, tài liệu đã đọc và điều bạn hiểu khác đi sau khi xem lại.

- Sau khi đọc, tham khảo tài liệu thì em đã hiểu rõ hơn về luồng chạy và xử lý lỗi vd: phân biệt khi nào nên dùng transaction, inner join - left join, xử lý trường hợp null, kiểu trả về,...

## 3. Kinh nghiệm rút ra

Nếu làm lại Phần 1, bạn sẽ thay đổi điều gì trong cách đọc đề, chia thời gian, tổ chức code hoặc kiểm tra kết quả? Nêu lý do.

- Làm lại đề em nhận ra không nên dành quá nhiều thời gian cho 1 bài tập để xử lý lỗi, nên đọc kỹ qua các yêu cầu và không đọc đến đâu làm tới đó, chia thời gian phù hợp để xử lý những câu dễ trước, dành thời gian cuối kiểm tra kỹ trường hợp đầu vào 1 lượt

## 4. Tự đánh giá

Nêu điểm bạn làm tốt, hạn chế hiện tại và kiến thức bạn vẫn chưa hiểu chắc. Leader sẽ đối chiếu báo cáo với source code và trao đổi trực tiếp với bạn.

- Điểm tốt của em là hiểu luồng code chạy hơn 2 bạn nên gặp một số lỗi và biết cách sửa, hạn chế hiện tại là syntax và code không sạch