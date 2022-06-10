function trim(str) {
    str = str.replace(/^\s+/, '');
    for (var i = str.length - 1; i >= 0; i--) {
        if (/\S/.test(str.charAt(i))) {
            str = str.substring(0, i + 1);
            break;
        }
    }
    return str;
}

jQuery.fn.dataTableExt.oSort['date-euro-asc'] = function (a, b) {
    if (trim(a) != '') {
        var frDatea = trim(a).split(' ');
        var frDatea2 = frDatea[0].split('/');
        var x = (frDatea2[2] + frDatea2[1] + frDatea2[0]) * 1;
    } else {
        var x = 10000000000000; // = l'an 1000 ...
    }

    if (trim(b) != '') {
        var frDateb = trim(b).split(' ');
        frDateb = frDateb[0].split('/');
        var y = (frDateb[2] + frDateb[1] + frDateb[0]) * 1;
    } else {
        var y = 10000000000000;
    }
    var z = ((x < y) ? -1 : ((x > y) ? 1 : 0));
    return z;
};

$.validator.methods.date = function (value, element) {
    if (value) {
        try {
            /*
            return moment(value, 'DD/MM/YYYY', true).isValid();
            */
            $.datepicker.parseDate('dd/mm/yy', value);
        } catch (ex) {
            return false;
        }
    }
    return true;
};

var mytt;
$(document).ready(function () {
    $.ajaxSetup({ cache: false })
    $('body').on('hidden.bs.modal', '#modal', function () {
        debugger;
        $(".modal-content", this).html(getLoadingHtml());
        $(this).removeData('bs.modal');
        $(this).data('bs.modal', null);
    });
});

function ServiceOrderEdit_OnFailure() {
    debugger;
    window.location.href = "/Account/Login?expired=true";
}
function ServiceOrderEdit_OnSuccess(data) {
    $(".homeDetail").html(data);
    $(".homeLoading").hide();
}

function ServiceOrderEdit_OnBegin() {
    $(".homeDetail").html("");
    //$("#homeLoadingSpan").html("");
    $(".homeLoading").show();
}

function getLoadingHtml() {
    return '<div style="line-height:100px;font-size:20px;min-height:100px;"><i class="fa fa-circle-o-notch fa-spin"></i> Loading...</div>';
}

function showConfirm(message, callback, callbackPars) {
    $(".modal-body", "#confirmModal").html(message);
    if (callback == undefined) {
        $("#btn_confirm_modal_no").hide();
        $("#btn_confirm_modal").html("OK");
    }
    else {
        $("#btn_confirm_modal_no").show();
        $("#btn_confirm_modal").html("SI");
    }

    $("#confirmModal").off("click", '#btn_confirm_modal');
    $('#confirmModal').modal({ backdrop: 'static', keyboard: false })
            .one('click', '#btn_confirm_modal', function (e) {
                if (callback != undefined) {
                    if (callbackPars == undefined) {
                        callback();
                    }
                    else {
                        callback(callbackPars);
                    }
                }
            });
}

function showMessage(data) {
    if (data.Message) {
        var div = $("<div/>");
        div.addClass("callout");
        var h4 = $("<h4/>");
        var p = $("<p/>");
        p.html(data.Message);
        if (data.Success) {
            div.addClass("callout-success");
            h4.html("Operazione effettuata con successo");
        }
        else {
            div.addClass("callout-danger");
            h4.html("Attenzione impossibile effettuare l'operazione");
        }
        //div.append(h4);
        div.append(p);
        div.attr("style", "display:none;");
        $("#message_container").html("");
        $("#message_container").append(div);

        $(div).fadeIn(2000);
        setTimeout(function () {
            $(div).fadeOut(2000);
        }, 2000);
    }
}

function ModalCallback_OnSuccess(data) {
    if (data.JavascriptCallback) {
        eval(data.JavascriptCallback)();
    }

    if (data.CloseDialog) {
        $(data.CloseDialog).modal('hide');
    }
    if (data.Selector) {
        $(data.Selector).html(data.Html);
    }

    if (data.Message) {
        showMessage(data);
    }
}

function SendPortfolio_OnBegin() {
    var html = '<i class="fa fa-refresh fa-spin" style="font-size:150px;margin-top:100px;margin-bottom:50px;margin-left:350px;"></i><br /><br /><span id="homeLoadingSpan" style="font-size:20px;"></span>';
    
    $(".modal-body", "#modalContent").html(
        html

        );
    debugger;
    $(".modal-footer", "#modalContent").remove();
}

function SendPortfolio_OnSuccess(data) {
    /*
    if (data.CloseDialog) {
        $(data.CloseDialog).modal('hide');
    }
    if (data.Message) {
        showMessage(data);
    }*/
    $("#modalContent").html(data.Html);
}

function initAutocompleteCodiceArticolo(selector, idSelector, emptyMessage,triggerKeyUp) {
    if (emptyMessage == undefined) {
        emptyMessage = "";
    }
    $("span", $(selector).closest(".input-group")).click(function () {
        var myInput = $("input", $(this).closest(".input-group"));
        myInput.removeAttr("disabled");
        myInput.val("");
        if ($("#lblCountry").length > 0) {
            $("#lblCountry").html("");
        }
        $(idSelector).val("");

        if (triggerKeyUp) {
            $(idSelector).trigger("keyup");
        }
        
    });
    $(selector).unbind();
    $(selector).typeahead({
        displayKey: 'CodiceArticolo',
        maxItem: 0, minLength: 2, hint: true,
        order: "asc", searchOnFocus: true, dynamic: true,
        highlight: false,
        template: '<div class="row">' +
                 '<div class="col-sm-12">{{CodiceArticolo}}</div>' +
                 '<div class="col-sm-12">{{Descrizione}}</div>' +
          '</div>',
        emptyTemplate: emptyMessage,
        source: {
            user: {
                display: ["CodiceArticolo", "Descrizione"],
                url: [
                    {
                        type: "GET",
                        url: '/Warehouse/GetArticoli',
                        data: {
                            query: '{{query}}'
                        },
                        callback: {
                            done: function (data) {
                                $(selector).removeAttr('style');
                                return data;
                            }
                            , fail: function () {
                                //window.location.href = "/Account/Login?expired=true";
                            }
                    }
                }]
            }
        },
        callback: {
            onClickAfter: function (node, a, item, event) {
                $(idSelector).val(item.CodiceArticolo);
                $(selector).val(item.CodiceArticolo);
                $(selector).attr("disabled", "disabled");
                if (triggerKeyUp) {
                    $(idSelector).trigger("keyup");
                }
            },
            onSendRequest: function (node, query) {
                $(selector).attr('style', 'background:url(\'/Content/Images/ajax-loader_OLD.gif\') no-repeat right center');
            },
            onReceiveRequest: function (node, query) { },
            onLayoutBuiltAfter: function (node, query) {
                $(".auto_tr_table").first().show();
            },
        },
        debug: false
    });

    $(selector).keypress(function (e) {
        if (e.which == 13) return false;
    });
}



function initAutocompleteDescrizioneArticolo(selector, idSelector, emptyMessage, triggerKeyUp) {
    if (emptyMessage == undefined) {
        emptyMessage = "";
    }
    $("span", $(selector).closest(".input-group")).click(function () {
        var myInput = $("input", $(this).closest(".input-group"));
        myInput.removeAttr("disabled");
        myInput.val("");
        $(idSelector).val("");

        if (triggerKeyUp) {
            $(idSelector).trigger("keyup");
        }

    });
    $(selector).unbind();
    $(selector).typeahead({
        displayKey: 'Descrizione',
        maxItem: 0, minLength: 2, hint: true,
        order: "asc", searchOnFocus: true, dynamic: true,
        highlight: false,
        template: '<div class="row">' +            
            '<div class="col-sm-12">{{Descrizione}}</div>' +
            '</div>',
        emptyTemplate: emptyMessage,
        source: {
            user: {
                display: ["Descrizione"],
                url: [
                    {
                        type: "GET",
                        url: '/Warehouse/GetDescrizioni',
                        data: {
                            query: '{{query}}'
                        },
                        callback: {
                            done: function (data) {
                                $(selector).removeAttr('style');
                                return data;
                            }
                            , fail: function () {
                                //window.location.href = "/Account/Login?expired=true";
                            }
                        }
                    }]
            }
        },
        callback: {
            onClickAfter: function (node, a, item, event) {
                $(idSelector).val(item.Descrizione);
                $(selector).val(item.Descrizione);
                $(selector).attr("disabled", "disabled");
                if (triggerKeyUp) {
                    $(idSelector).trigger("keyup");
                }
            },
            onSendRequest: function (node, query) {
                $(selector).attr('style', 'background:url(\'/Content/Images/ajax-loader_OLD.gif\') no-repeat right center');
            },
            onReceiveRequest: function (node, query) { },
            onLayoutBuiltAfter: function (node, query) {
                $(".auto_tr_table").first().show();
            },
        },
        debug: false
    });

    $(selector).keypress(function (e) {
        if (e.which == 13) return false;
    });
}






