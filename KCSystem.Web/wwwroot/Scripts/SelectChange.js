var mysel = $("#Progress");
var ProgressList = ["一审", "二审", "再审"];
$(function () {
    mysel.data("last", mysel.val()).change(function () {
        var oldvalue = mysel.data("last"); //这次改变之前的值
        mysel.data("last", mysel.val()); //每次改变都附加上去，以便下次变化时获取
        var newvalue = mysel.val(); //当前选中值
        if ((newvalue == "执行" && ProgressList.includes(oldvalue)) || (ProgressList.includes(newvalue) && oldvalue == "执行")) {
            GetCaseLinkDropdown();
            $("#CaseLinkId").val("").trigger("change");
        }
    });
    GetCaseLinkDropdown();
    $("#CaseLinkId").bind("change",
        function () {
            $("#CaseLinkName").val($("#CaseLinkId").select2('data')[0].text);
        });
});

function GetCaseLinkDropdown() {
    $("#CaseLinkId").select2({
            width: "100%",
            ajax: {
                delay: 250,
                url: '/CasesLink/GetCaseLinkDropdown',
                data: function (params) {
                    var query = {
                        name: $("#Progress").val(),
                        search: params.term,
                        type: 'public'
                    }
                    return query;
                }
            },
            placeholder: '请选择', //默认文字提示
            language: "zh-CN",
            allowClear: true, //允许清空
            escapeMarkup: function (markup) { return markup; }, // 自定义格式化防止xss注入
            minimumResultsForSearch: -1
        },
        true);
}