$(document).ready(function () {
    $('#LoginTutorialModal').on('hidden.bs.modal', function (e) {
        $("#LoginTutorialModal iframe").attr("src", $("#LoginTutorialModal iframe").attr("src"))
    })
    $('#WarehouseTutorialModal').on('hidden.bs.modal', function (e) {
        $("#WarehouseTutorialModal iframe").attr("src", $("#WarehouseTutorialModal iframe").attr("src"))
    })
    $('#SalesTutorialModal').on('hidden.bs.modal', function (e) {
        $("#SalesTutorialModal iframe").attr("src", $("#SalesTutorialModal iframe").attr("src"))
    })
    $('#ConsTutorialModal').on('hidden.bs.modal', function (e) {
        $("#ConsTutorialModal iframe").attr("src", $("#ConsTutorialModal iframe").attr("src"))
    })

});