console.log("connect Script Success");
$('input[type = "checkbox"][name = "sizes"]').on('change', function () {
	var selectSizes = $('input[type = "checkbox"][name = "sizes"]:checked').map(function () {
		return $(this).val();
	}).get();
	console.log(selectSizes);
	$.ajax({
		//url: '@Url.Action("GetFilteredProducts", "Product")',
		url: '/Product/GetFilteredProducts',
		type: 'POST',
		dataType: 'html',
		data: {
			sizeIds: selectSizes
		},
		success: function (result) {
			//$('#product-list').html(result);
			$('#CardProduct-Filter').html(result);
		},
		error: function () {
			console.log('An error while get filtered');
		}
	});
});