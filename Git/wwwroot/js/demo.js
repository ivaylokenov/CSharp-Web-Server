$(document).ready(function() {
  // javascript
  var slider = document.getElementById('demo-slider');
  noUiSlider.create(slider, {
      start: '20',
      connect: [true, false],
      range: {
          'min': 0,
          'max': 100
      }
  });

  // javascript
  var slider_handles = document.getElementById('demo-slider-handles');
  noUiSlider.create(slider_handles, {
      start: ['20', '40'],
      connect: !0,
      range: {
          'min': 0,
          'max': 100
      }
  });

});
