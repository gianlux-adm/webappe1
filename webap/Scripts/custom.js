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

function isNumberKey(txt, evt) {
    if (evt.keyCode == '44' || evt.charCode == '44') {
        if (txt.value.indexOf('.') === -1) {
        } else {
            return false;
        }

        // IE
        if (document.selection) {
            // Determines the selected text. If no text selected,
            // the location of the cursor in the text is returned
            var range = document.selection.createRange();
            // Place the comma on the location of the selection,
            // and remove the data in the selection
            range.text = '.';
            // Chrome + FF
        } else if ($(txt).selectionStart || $(txt).selectionStart == '0') {
            // Determines the start and end of the selection.
            // If no text selected, they are the same and
            // the location of the cursor in the text is returned
            // Don't make it a jQuery obj, because selectionStart
            // and selectionEnd isn't known.
            var start = $(txt).selectionStart;
            var end = $(txt).selectionEnd;
            // Place the comma on the location of the selection,
            // and remove the data in the selection
            $(txt).val($(txt).val().substring(0, start) + '.' + $(txt).val().substring(end, $(txt).val().length));
            // Set the cursor back at the correct location in
            // the text
            $(txt).selectionStart = start + 1;
            $(txt).selectionEnd = start + 1;
        } else {
            // if no selection could be determined,
            // place the comma at the end.
            $(txt).val($(txt).val() + '.');
        }

        if (txt.value.indexOf('.') === -1) {
            return true;
        } else {
            return false;
        }
    }
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 46) {
        //Check if the text already contains the . character
        if (txt.value.indexOf('.') === -1) {
            return true;
        } else {
            return false;
        }
    } else {
        if (charCode > 31 &&
            (charCode < 48 || charCode > 57))
            return false;
    }
    return true;
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

function initAutocompleteCodiceArticoloOrdine(selector, idSelector, emptyMessage, triggerKeyUp, mode) {
    if (emptyMessage == undefined) {
        emptyMessage = "";
    }
    $(".input-group-addon", $(selector).closest(".input-group")).click(function () {
        var myInput = $("input", $(this).closest(".input-group"));
        myInput.removeAttr("disabled");
        myInput.val("");

        $(".typeahead-list", $(this).closest(".typeahead-container")).remove();
        $(".typeahead-hint", $(this).closest(".typeahead-container")).remove();
        if ($("#lblCountry").length > 0) {
            $("#lblCountry").html("");
        }
        $(idSelector).val("");

        if (triggerKeyUp) {
            $(idSelector).trigger("keyup");
        }

        $("#UnitaMisura").val("");
        $("#UnitaMisuraLabel").html("");

        $("#StatoDazio").attr("disabled", "disabled");
        $("#StatoDazio").val("");
    });
    var display = ["CodiceArticolo", "Descrizione"];
    var template = '<div class="row">' +
        '<div class="col-sm-12">{{CodiceArticolo}}</div>' +
        '<div class="col-sm-12">{{Descrizione}}</div>' +
        '</div>';
    var endpoint = '/Orders/GetArticoli';

    $(selector).unbind();
    $(selector).typeahead({
        displayKey: 'CodiceArticolo',
        delay: 1000,
        maxItem: 0, minLength: 2, hint: true,
        order: "asc", searchOnFocus: true, dynamic: true,
        highlight: false,
        template: template,
        emptyTemplate: emptyMessage,
        source: {
            user: {
                display: display,
                url: [
                    {
                        type: "GET",
                        url: endpoint,
                        data: {
                            query: '{{query}}',
                            type: function () { return $("#type_CodiceArticoloText").attr("data-type") }
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
                $("#UnitaMisura").val(item.UM);
                $("#UnitaMisuraLabel").html(item.UM);

                if (item.DTYS == "SY") {
                    $("#StatoDazio").removeAttr("disabled");
                }
                else {
                    $("#StatoDazio").attr("disabled", "disabled");
                    $("#StatoDazio").val("");
                }

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

function initAutocompleteCodiceArticolo(selector, idSelector, emptyMessage, triggerKeyUp, mode) {
    if (emptyMessage == undefined) {
        emptyMessage = "";
    }
    $(".input-group-addon", $(selector).closest(".input-group")).click(function () {
        var myInput = $("input", $(this).closest(".input-group"));
        myInput.removeAttr("disabled");
        myInput.val("");
        $(".typeahead-list", $(this).closest(".typeahead-container")).remove();
        $(".typeahead-hint", $(this).closest(".typeahead-container")).remove();
        if ($("#lblCountry").length > 0) {
            $("#lblCountry").html("");
        }
        $(idSelector).val("");

        if (triggerKeyUp) {
            $(idSelector).trigger("keyup");
        }
    });
    var display = ["CodiceArticolo", "Descrizione"];
    var template = '<div class="row">' +
        '<div class="col-sm-12">{{CodiceArticolo}}</div>' +
        '<div class="col-sm-12">{{Descrizione}}</div>' +
        '</div>';
    var endpoint = '/Warehouse/GetArticoli';
    if (mode != undefined && mode == "Articolo") {
        display = ["CodiceArticolo"];
        template = '<div class="row">' +
            '<div class="col-sm-12">{{CodiceArticolo}}</div>' +
            '</div>';
        endpoint = '/Warehouse/GetArticoliPerListino'
    }
    $(selector).unbind();
    $(selector).typeahead({
        displayKey: 'CodiceArticolo',
        delay: 1000,
        maxItem: 0, minLength: 2, hint: true,
        order: "asc", searchOnFocus: true, dynamic: true,
        highlight: false,
        template: template,
        emptyTemplate: emptyMessage,
        source: {
            user: {
                display: display,
                url: [
                    {
                        type: "GET",
                        url: endpoint,
                        data: {
                            query: '{{query}}',
                            type: function () { return $("#type_CodiceArticoloText").attr("data-type") }
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
                debugger;
                $(idSelector).val(item.CodiceArticolo);
                $(selector).val(item.CodiceArticolo + " - " + item.Descrizione);
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

function initAutocompleteCliente(selector, idSelector, emptyMessage, triggerKeyUp) {
    if (emptyMessage == undefined) {
        emptyMessage = "";
    }
    $(".input-group-addon", $(selector).closest(".input-group")).click(function () {
        var myInput = $("input", $(this).closest(".input-group"));
        myInput.removeAttr("disabled");
        myInput.val("");
        $(".typeahead-list", $(this).closest(".typeahead-container")).remove();
        $(".typeahead-hint", $(this).closest(".typeahead-container")).remove();
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
        displayKey: 'CodiceCliente',
        delay: 1000,
        maxItem: 0, minLength: 2, hint: true,
        order: "asc", searchOnFocus: true, dynamic: true,
        highlight: false,
        template: '<div class="row">' +
            '<div class="col-sm-12">{{CodiceCliente}}</div>' +
            '<div class="col-sm-12">{{DescrizioneCliente}}</div>' +
            '</div>',
        emptyTemplate: emptyMessage,
        source: {
            user: {
                filter: false,
                display: ["CodiceCliente", "DescrizioneCliente"],
                url: [
                    {
                        type: "GET",
                        url: '/Sales/GetClienti',
                        data: {
                            query: '{{query}}'
                        },
                        callback: {
                            done: function (data) {
                                debugger;
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
                debugger;
                $(idSelector).val(item.CodiceCliente);
                $(selector).val(item.CodiceCliente + " - " + item.DescrizioneCliente);
                $(selector).attr("disabled", "disabled");
                if (triggerKeyUp) {
                    $(idSelector).trigger("keyup");
                }
            },
            onSendRequest: function (node, query) {
                $(selector).attr('style', 'background:url(\'/Content/Images/ajax-loader_OLD.gif\') no-repeat right center');
            },
            onReceiveRequest: function (node, query) {
                debugger;
            },
            onLayoutBuiltAfter: function (node, query) {
                debugger;
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
        $(".typeahead-list", $(this).closest(".typeahead-container")).remove();
        $(".typeahead-hint", $(this).closest(".typeahead-container")).remove();

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
        delay: 1000,
        highlight: false,
        template: '<div class="row">' +
            '<div class="col-sm-12">{{CodiceArticolo}}</div>' +
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

function initAutocompleteUser(selector, idSelector, emptyMessage) {
    $("span", $(selector).closest(".input-group")).click(function () {
        var myInput = $("input", $(this).closest(".input-group"));
        myInput.removeAttr("disabled");
        myInput.val("");
        $(idSelector).val("");
    });
    $(selector).unbind();
    $(selector).typeahead({
        displayKey: 'UserId',
        maxItem: 0, minLength: 2, hint: true,
        order: "asc", searchOnFocus: true, dynamic: true,
        highlight: true,
        template: '<div class="row">' +
            '<div class="col-sm-12">{{Name}}</div>' +
            '<div class="col-sm-12">{{LoginName}} ({{RoleDescription}})</div>' +
            '</div>',
        emptyTemplate: emptyMessage,
        source: {
            user: {
                display: ["UserId", "Name", "LoginName", "RoleDescription"],
                url: [{
                    type: "GET",
                    url: '/Log/User',
                    data: {
                        query: '{{query}}'
                    },
                    callback: {
                        done: function (data) {
                            $(selector).removeAttr('style');
                            return data;
                        }, fail: function () {
                            window.location.href = "/Account/Login?expired=true";
                        }
                    }
                }]
            }
        },
        callback: {
            onClickAfter: function (node, a, item, event) {
                $(idSelector).val(item.UserId);
                $(selector).val(item.LoginName + " - " + item.RoleDescription);
                $(selector).attr("disabled", "disabled");
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