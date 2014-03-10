﻿//Start AbsPerformances section//////////////////////////
//Add AbsPerformance modal 
var AddAbsPerformanceViewModel = function() {

    var self = this;

    edit_abs_track_performance_id = typeof(edit_abs_track_performance_id) !== 'undefined' ? edit_abs_track_performance_id : 0;
    this.edit_abs_track_performance_id = ko.observable(edit_abs_track_performance_id);

    edit_abs_measure_type_id = typeof(edit_abs_measure_type_id) !== 'undefined' ? edit_abs_measure_type_id : 1; //default value
    this.edit_abs_measure_type_id = ko.observable(1);

    this.edit_abs_return_apl_func = ko.observable("");

    edit_abs_currency_id = typeof(edit_abs_currency_id) !== 'undefined' ? edit_abs_currency_id : ""; //default value
    this.edit_abs_currency_id = ko.observable(edit_abs_currency_id).extend({
        required: {
            onlyIf: function() {
                return self.edit_abs_measure_type_id() == 2;
            },
        }
    });

    this.edit_abs_return_benchmark_id = ko.observable(0);

    edit_abs_return_value = typeof(edit_abs_return_value) !== 'undefined' ? edit_abs_return_value : "";
    this.edit_abs_return_value = ko.validatedObservable(edit_abs_return_value).extend({ required: true }).extend({ number: true });

    this.edit_abs_return_date = ko.validatedObservable("").extend({ required: true, dateISO: true });

};

// The name of the template to render
AddAbsPerformanceViewModel.prototype.template = "AddAbsPerformance";

AddAbsPerformanceViewModel.prototype.add = function () {
    var newAbsPerformance = {
        edit_abs_track_performance_id: this.edit_abs_track_performance_id(),

        edit_abs_measure_type_id: this.edit_abs_measure_type_id(),

        edit_abs_return_apl_func: this.edit_abs_return_apl_func(),

        edit_abs_currency_id: this.edit_abs_currency_id(),

        edit_abs_return_benchmark_id: this.edit_abs_return_benchmark_id(),

        edit_abs_return_value: this.edit_abs_return_value(),

        edit_abs_return_date: this.edit_abs_return_date()
    };
    var result = ko.validation.group(this, { deep: true });
    if (this.isValid()) {
        // Close the modal, passing the new note object as the result data.
        this.modal.close(newAbsPerformance);
    } else {
        result.showAllMessages(true);
    }
    
};

AddAbsPerformanceViewModel.prototype.cancel = function () {
    // Close the modal without passing any result data.
    this.modal.close();
};

vm.addAbsPerformance = function () {
    showModal({
        viewModel: new AddAbsPerformanceViewModel(),
        context: this // Set context so we don't need to bind the callback function
    }).then(this._addAbsPerformanceToAbsPerformances);
};

vm._addAbsPerformanceToAbsPerformances = function (newAbsPerformance) {
    console.log('saving added AbsPerformance');
    this.edit_abs_track_performance_id(0);
    this.edit_abs_measure_type_id(newAbsPerformance.edit_abs_track_performance_id);
    this.edit_abs_return_apl_func(newAbsPerformance.edit_abs_track_performance_id);
    this.edit_abs_currency_id(newAbsPerformance.edit_abs_track_performance_id);
    this.edit_abs_return_benchmark_id(newAbsPerformance.edit_abs_track_performance_id);
    this.edit_abs_return_value(newAbsPerformance.edit_abs_track_performance_id);
    this.edit_abs_return_date(newAbsPerformance.edit_abs_track_performance_id);

    vm.Save_AbsPerformance_Edit_RecordnewAbsPerformance();

};
//End Add AbsPerformance modal 

//Edit AbsPerformance modal 
var EditAbsPerformanceViewModel = function () {

    var self = this;
    
    edit_abs_track_performance_id = typeof(edit_abs_track_performance_id) !== 'undefined' ? edit_abs_track_performance_id : 0;
    this.edit_abs_track_performance_id = ko.observable(edit_abs_track_performance_id);

    edit_abs_measure_type_id = typeof(edit_abs_measure_type_id) !== 'undefined' ? edit_abs_measure_type_id : 1; //default value
    this.edit_abs_measure_type_id = ko.observable(1);

    this.edit_abs_return_apl_func = ko.observable("");

    edit_abs_currency_id = typeof(edit_abs_currency_id) !== 'undefined' ? edit_abs_currency_id : ""; //default value
    this.edit_abs_currency_id = ko.observable(edit_abs_currency_id).extend({
        required: {
            onlyIf: function() {
                return self.edit_abs_measure_type_id == 2;
            },
        }
    });

    this.edit_abs_return_benchmark_id = ko.observable(0);

    edit_abs_return_value = typeof(edit_abs_return_value) !== 'undefined' ? edit_abs_return_value : "";
    this.edit_abs_return_value = ko.validatedObservable(edit_abs_return_value).extend({ required: true }).extend({ number: true });

    this.edit_abs_return_date = ko.validatedObservable("").extend({ required: true, dateISO: true });

    this.abs_Performanceitems = ko.observableArray([]);

    this.abs_SelectedItems = ko.observableArray([]);

    this.abs_SelectedItems.subscribe(
        function() {
            var recordindex = (self.abs_SelectedItems().length) - 1;
            if (recordindex >= 0) {
                var record = self.abs_SelectedItems()[recordindex];
                self.edit_abs_track_performance_id(record.trade_performance_id);
                self.edit_abs_measure_type_id(record.measure_type_id);
                self.edit_abs_return_apl_func(record.return_apl_function);
                self.edit_abs_currency_id(record.return_currency_id);
                //self.edit_abs_return_benchmark_id();
                self.edit_abs_return_value(record.return_value);
                self.edit_abs_return_date(record.return_date);
                $('#modalEditAbsCurrencyTypeAhead').trigger('change');
            } else {
                self.edit_abs_track_performance_id(0);
                self.edit_abs_measure_type_id(1);
                //self.edit_abs_return_apl_func(record.return_apl_function);
                self.edit_abs_currency_id("");
                //self.edit_abs_return_benchmark_id();
                self.edit_abs_return_value("");
                self.edit_abs_return_date("");
                $('#modalEditAbsCurrencyTypeAhead').trigger('change');
            }
        });
    
    self.abs_Performanceitems.removeAll();
    self.abs_SelectedItems.removeAll();
    var apiGetURL = baseUrl;
    apiGetURL += "Trade/GetAbsolutePerformances/";
    apiGetURL += vm.trade_id();

    $.getJSON(apiGetURL, function (allData) {
        self.abs_Performanceitems(allData);
    });
};

// The name of the template to render
EditAbsPerformanceViewModel.prototype.template = "EditAbsPerformance";

EditAbsPerformanceViewModel.prototype.save = function () {
    var editedAbsPerformance = {
        edit_abs_track_performance_id: this.edit_abs_track_performance_id(),

        edit_abs_measure_type_id: this.edit_abs_measure_type_id(),

        edit_abs_return_apl_func: this.edit_abs_return_apl_func(),

        edit_abs_currency_id: this.edit_abs_currency_id(),

        edit_abs_return_benchmark_id: this.edit_abs_return_benchmark_id(),

        edit_abs_return_value: this.edit_abs_return_value(),

        edit_abs_return_date: this.edit_abs_return_date()
    };
    if (self.abs_edit_isValid()) {
        // Close the modal, passing the new note object as the result data.
        this.modal.close(editedAbsPerformance);
    }
};

EditAbsPerformanceViewModel.prototype.cancel = function () {
    // Close the modal without passing any result data.
    this.modal.close();
};

vm.editAbsPerformance = function () {
    var viewModel = new EditAbsPerformanceViewModel();
   
    
    showModal({
        viewModel: viewModel,
        context: this // Set context so we don't need to bind the callback function
    }).then(this._editAbsPerformanceToAbsPerformances);
};

vm._editAbsPerformanceToAbsPerformances = function (editedAbsPerformance) {
    console.log('saving edited AbsPerformance');
    //self.AbsPerformance_id(editedAbsPerformance.id);
    //self.AbsPerformances(editedAbsPerformance.text);
    //self.saveAbsPerformanceData();
};
//End Edit AbsPerformance modal
abs_edit_isValid = function (viewmodel) {
    return viewmodel.edit_abs_track_performance_id.isValid() &&
        viewmodel.edit_abs_measure_type_id.isValid() &&
        //viewmodel.edit_abs_return_apl_func(record.return_apl_function);
        viewmodel.edit_abs_currency_id.isValid() &&
        //viewmodel.edit_abs_return_benchmark_id();
        viewmodel.edit_abs_return_value.isValid() &&
        viewmodel.edit_abs_return_date.isValid();
};
//End AbsPerformances section//////////////////////////

Save_AbsPerformance_Edit_Record = function (viewmodel) {
        console.log('Posting AbsolutePerformances to server to save.');
        var apiURL = baseUrl;
        apiURL += "api/AbsolutePerformances/post/";
        $.ajax({
            url: apiURL,
            type: 'post',
            contentType: 'application/json',
            async: false,
            data: JSON.stringify({
                "trade_performance_id": viewmodel.edit_abs_track_performance_id(),
                "trade_id": viewmodel.trade_id(),
                "measure_type_id": viewmodel.edit_abs_measure_type_id(),
                "return_currency_id": viewmodel.edit_abs_currency_id(),
                "return_value": viewmodel.edit_abs_return_value(),
                "return_date": viewmodel.edit_abs_return_date()
            }),
            success: function (data) {
                if (data.Success) {
                    //update trade info
                    viewmodel.abs_Performanceitems.removeAll();
                    viewmodel.abs_SelectedItems.removeAll();
                    var apiGetURL = baseUrl;
                    apiGetURL += "Trade/GetAbsolutePerformances/";
                    apiGetURL += self.trade_id();

                    $.getJSON(apiGetURL, function (allData) {
                        viewmodel.abs_Performanceitems(allData);
                    });
                    afterModalEditLoadData();
                    window.onbeforeunload = null;
                    console.log(data.Message);
                    bootbox.dialog({
                        message: data.Message,
                        title: "Absolute Performance",
                        buttons: {
                            success: {
                                label: "OK",
                                className: "btn-success",
                                callback: function () {
                                    return true;
                                }
                            },
                        }
                    });
                } else {
                    console.log(data.Message);
                    bootbox.alert(data.Message); //display exception

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log("error: " + XMLHttpRequest.responseText);
                var message = "error: " + XMLHttpRequest.responseText;
                bootbox.dialog({
                    message: message,
                    title: "Absolute Performance",
                    className: "alert-danger",
                    buttons: {
                        danger: {
                            label: "OK",
                            className: "btn-danger",
                            callback: function () {
                            }
                        }
                    }
                });
            }
        });
   
};