$(document).ready(function () {
  //some TestTEst // 中亠方顺时而动加工 
   //var timer = $.timer(TimerTick);
  // timer.set({ time: 5000, autostart: true });
  // LoadMeterData();

   // $("#server_time").html("+0000000222222222");
});


function TimerTick() {
    LoadMeterData();
}

function LoadMeterData() {

    $.getScript(_base_url + "/Nx/TestController/AjaxGetServerTime")
    .done(function (script, textStatus) {

        console.log(_dateTimeValue);
        $("#server_time").html(_dateTimeValue+"123456798");

     })
    .fail(function (jqxhr, settings, exception) {

    });


   

   
}