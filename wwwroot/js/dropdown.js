$(document).ready(function () {
    $('.category-checkbox').change(function () {
        var selectedCategories = [];
        $('.category-checkbox:checked').each(function () {
            selectedCategories.push($(this).val());
        });

        $.ajax({
            url: '/api/products', // URL ของ API ที่ต้องการเรียกใช้
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ typeIds: selectedCategories }), // ส่งข้อมูล checkbox ไปที่ API
            success: function (data) {
                // เรียกฟังก์ชันสำหรับการแสดงผลสินค้า
                renderProducts(data);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error:', errorThrown);
            }
        });
    });
});

// ฟังก์ชันสำหรับแสดงผลสินค้า
function renderProducts(products) {
    $('#CardProduct-Filter').empty(); // ลบสินค้าที่แสดงอยู่ในปัจจุบัน
    products.forEach(function (item) {
        var pdimg = "/images/" + item.ProductId + ".png";
        var productHtml = `
                <div class="col-xl-3 col-lg-4 col-md-4 col-sm-6 mt-3 product-card" data-category="${item.CategoryName}">
                    <div class="card px-3 py-3">
                        <img src="${pdimg}" class="card-img-top img-fluid object-fit-cover" style="height:30vh" alt="...">
                        <hr />
                        <div class="card-body">
                            <h5 class="card-title text-truncate">${item.ProductName}</h5>
                            <a href="/Product/Show/${item.ProductId}" class="btn btn-btn-link">รายละเอียด</a>
                            <a href="/Cart/AddDtl/${item.ProductId}?qty=1" class="text-danger float-end"><i class="bi bi-cart-plus-fill"></i></a>
                        </div>
                    </div>
                </div>`;
        $('#CardProduct-Filter').append(productHtml); // เพิ่มสินค้าใหม่ลงในส่วนของ HTML
    });
}