$(document).ready(function () {
    $('.category-checkbox').change(function () {
        var selectedCategories = [];
        $('.category-checkbox:checked').each(function () {
            selectedCategories.push($(this).val());
        });

        $.ajax({
            url: '/api/products', // URL �ͧ API ����ͧ������¡��
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ typeIds: selectedCategories }), // �觢����� checkbox 价�� API
            success: function (data) {
                // ���¡�ѧ��ѹ����Ѻ����ʴ����Թ���
                renderProducts(data);
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error:', errorThrown);
            }
        });
    });
});

// �ѧ��ѹ����Ѻ�ʴ����Թ���
function renderProducts(products) {
    $('#CardProduct-Filter').empty(); // ź�Թ��ҷ���ʴ�����㹻Ѩ�غѹ
    products.forEach(function (item) {
        var pdimg = "/images/" + item.ProductId + ".png";
        var productHtml = `
                <div class="col-xl-3 col-lg-4 col-md-4 col-sm-6 mt-3 product-card" data-category="${item.CategoryName}">
                    <div class="card px-3 py-3">
                        <img src="${pdimg}" class="card-img-top img-fluid object-fit-cover" style="height:30vh" alt="...">
                        <hr />
                        <div class="card-body">
                            <h5 class="card-title text-truncate">${item.ProductName}</h5>
                            <a href="/Product/Show/${item.ProductId}" class="btn btn-btn-link">��������´</a>
                            <a href="/Cart/AddDtl/${item.ProductId}?qty=1" class="text-danger float-end"><i class="bi bi-cart-plus-fill"></i></a>
                        </div>
                    </div>
                </div>`;
        $('#CardProduct-Filter').append(productHtml); // �����Թ�������ŧ���ǹ�ͧ HTML
    });
}