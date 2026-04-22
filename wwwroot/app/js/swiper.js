// ainimate sider
(function($) {
    var Spanizer = (function() {
      var settings = {
        letters: $('.js-letters'),
      };
      return {
        init: function() {
          this.bind();
        },
        bind: function(){
          Spanizer.doSpanize();
        },
        doSpanize: function(){
            settings.letters.html(function (i, el) {
            var spanize = $.trim(el).split("");
            var template = '<span>' + spanize.join('</span><span>') + '</span>'
            return template;
            });
        },
      };
    })();
    // Let's GO!
    Spanizer.init();
})(jQuery);


//  slider
var swiper =  new Swiper(".mainslider", {
    autoplay: {
    delay: 6000,
    disableOnInteraction: false,
    },
    slidesPerView: 1,
    speed: 500,
    effect: "fade",
    fadeEffect: {
        crossFade: true,
    },
    navigation: {
        nextEl: ".swiper-button-next5",
        prevEl: ".swiper-button-prev5",
    },
    pagination: {
        el: ".swiper-pagination5",
        clickable: true,
      
    },
});

var swiper =  new Swiper(".mainslider2", {
    autoplay: {
    delay: 6000,
    disableOnInteraction: false,
    },
    slidesPerView: 1,
    speed: 500,
    effect: "fade",
    fadeEffect: {
        crossFade: true,
    },
    navigation: {
        nextEl: ".swiper-button-next3",
        prevEl: ".swiper-button-prev3",
    },
    pagination: {
        el: ".swiper-pagination6",
        clickable: true,
    },
});


if ($('.one-carousel').length) {
    $('.one-carousel').owlCarousel({
        loop:false,
        margin:30,
        smartSpeed: 500,
        autoplay: 4000,
        responsive:{
            0:{
                items:1
            },
            480:{
                items:1
            },
            767:{
                items:2
            },
            996:{
                items:3
            }
        }
    });    		
}

if ($('.two-carousel').length) {
    $('.two-carousel').owlCarousel({
        loop:false,
        margin:30,
        smartSpeed: 500,
        autoplay: 4000,
        pagination: {
            clickable: true,
          },
        responsive:{
            0:{
                items:1
            },
            480:{
                items:1
            },
            767:{
                items:2
            },
            992:{
                items:3
            },
            1199:{
                items:4
            }
        }
    });    		
}

$('.three-carousel').owlCarousel({
    loop:false,
    margin:30,
    nav:true,
    smartSpeed: 500,
    autoplay: 4000,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next4",
        prevEl: ".swiper-button-prev4",
    },
    responsive:{
        0:{
            items:1
        },
        767:{
            items:2
        },
        996:{
            items:3
        },
        1300:{
            items:4
        },
        1600:{
            items:5
        }
    }
});    		

if ($('.four-carousel').length) {
    $('.four-carousel').owlCarousel({
        loop:false,
        margin:30,
        smartSpeed: 500,
        autoplay: 4000,
        responsive:{
            0:{
                items:1
            },
            767:{
                items:2
            },
            996:{
                items:2
            },
            1520:{
                items:3
            }
        }
    });    		
}

if ($('.sponsors-carousel').length) {
    $('.sponsors-carousel').owlCarousel({
        loop:false,
        margin:30,
        nav:true,
        smartSpeed: 500,
        autoplay: 4000,
        navigation: {
            clickable: true,
            nextEl: ".swiper-button-next4",
            prevEl: ".swiper-button-prev4",
        },
        responsive:{
            0:{
                items:1
            },
            767:{
                items:2
            },
            996:{
                items:3
            }
        }
    });    		
}

var swiper = new Swiper(".location-slider", {
    loop: true,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
    clickable: true,
    nextEl: ".button-lo-next",
    prevEl: ".button-lo-prev",
    },
    pagination: {
    el: ".swiper-pagination",
    clickable: true,
    
    },
    breakpoints: {
    600: {
        slidesPerView: 2,
        spaceBetween: 19.5,
    },
    992: {
        slidesPerView: 3,
        spaceBetween: 19.5,
    },
    1200: {
        slidesPerView: 3,
        spaceBetween: 30,
    },
    },
});

var swiper =  new Swiper(".carousel-1", {
    autoplay: {
        delay: 5000,
        disableOnInteraction: false,
        },
    loop:false,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    breakpoints: {
        768: {
            slidesPerView: 2,
            spaceBetween: 30,
        },
        1024: {
            slidesPerView: 3,
            spaceBetween: 30,
        },
        1300: {
            slidesPerView: 4,
            spaceBetween: 30,
        },
    },
});

var swiper =  new (this).Swiper(".carousel-2" , {
    // autoplay: {
    //     delay: 5000,
    //     disableOnInteraction: false,
    //     },
    loop:false,
    slidesPerView: 1,
    spaceBetween: 0,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next2",
        prevEl: ".swiper-button-prev2",
    },
    pagination: {
        el: ".swiper-pagination2",
        clickable: true,
    },
    breakpoints: {
        768: {
            slidesPerView: 1,
            spaceBetween: 0,
        },
        1200: {
            slidesPerView: 1,
            spaceBetween: 0,
        },
    },
});



var swiper =  new Swiper(".carousel-3", {
    autoplay: {
        delay: 6000,
        disableOnInteraction: false,
    },
    slidesPerView: 1,   
    loop: false,
    spaceBetween: 30,
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
      },
    breakpoints: {
        768: {
            slidesPerView: 1,
            spaceBetween: 30,
        },
        991: {
            slidesPerView: 3,
            spaceBetween: 30,
        },
    },
});

var swiper =  new Swiper(".carousel-4", {
    autoplay: {
        delay: 5000,
        disableOnInteraction: false,
    },
    
    slidesPerView: 1,   
    loop: false,
    spaceBetween: 30,
    // grabCursor: true,
    pagination: {
        el: ".swiper-pagination1",
        clickable: true,
      },
    breakpoints: {
        768: {
            slidesPerView: 2,
            spaceBetween: 29,
        },
        991: {
            slidesPerView: 3,
            spaceBetween: 29,
        },
        1200: {
            slidesPerView: 5,
            spaceBetween: 29,
        },
        1550: {
            slidesPerView: 7,
            spaceBetween: 29,
        },
    },
});

var swiper =  new Swiper(".carousel-5", {
    autoplay: {
        delay: 0,
        disableOnInteraction: false,
    },
    slidesPerView: 2,   
    loop: true,
    spaceBetween: 30,
    speed: 10000,
    observer: true,
    observeParents: true,
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    breakpoints: {
        450: {
            slidesPerView: 3,
            spaceBetween: 30,
        },
        768: {
            slidesPerView: 4,
            spaceBetween: 30,
        },
        868: {
            slidesPerView: 5,
            spaceBetween: 30,
        },
        992: {
            slidesPerView: 6,
            spaceBetween: 30,
        },
    },
});

var swiper =  new Swiper(".carousel-6", {
    autoplay: {
        delay: 5000,
        disableOnInteraction: false,
    },
    loop:true,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next4",
        prevEl: ".swiper-button-prev4",
    },
    breakpoints: {
        768: {
            slidesPerView: 2,
            spaceBetween: 30,
        },
        992: {
            slidesPerView: 3,
            spaceBetween: 30,
        },
    },
});
  

var swiper =  new Swiper(".carousel-7", {
    autoplay: {
        delay: 4000,
        disableOnInteraction: true,
    },
    loop:false,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next3",
        prevEl: ".swiper-button-prev3",
    },
    pagination: {
        el: ".swiper-pagination1",
        clickable: true,
    },
    breakpoints: {
        768: {
            slidesPerView: 1,
            spaceBetween: 30,
        },
        992: {
            slidesPerView: 1,
            spaceBetween: 30,
        },
    },
});

var swiper =  new Swiper(".carousel-8", {
    loop:false,
    slidesPerView: 2,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    breakpoints: {
        768: {
            slidesPerView: 4,
            spaceBetween: 30,
        },
        992: {
            slidesPerView: 6,
            spaceBetween: 30,
        },
    },
});

var swiper =  new Swiper(".carousel-9", {
    loop:false,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next4",
        prevEl: ".swiper-button-prev4",
    },
    pagination: {
        el: ".swiper-pagination1",
        clickable: true,
    },
    breakpoints: {
        768: {
            slidesPerView: 2,
            spaceBetween: 30,
        },
        992: {
            slidesPerView: 2,
            spaceBetween: 30,
        },

    },
});

var swiper =  new Swiper(".carousel-10", {
    loop:false,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    pagination: {
        el: ".swiper-pagination1",
        clickable: true,
    },
    breakpoints: {
        768: {
            slidesPerView: 3,
            spaceBetween: 27,
        },
        992: {
            slidesPerView: 4,
            spaceBetween: 27,
        },

    },
});

var swiper =  new Swiper(".carousel-11", {
    loop:false,
    slidesPerView: 1,
    spaceBetween: 30,
    navigation: {
        clickable: true,
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    pagination: {
        el: ".swiper-pagination1",
        clickable: true,
    },
    breakpoints: {
        768: {
            slidesPerView: 2,
            spaceBetween: 30,
        },
        992: {
            slidesPerView: 3,
            spaceBetween: 12,
        },

    },
});

var swiperthumbs = new Swiper(".thumbs-swiper-row1", {
    spaceBetween: 16,
    slidesPerView: "auto",
    freeMode: true,
    watchSlidesProgress: true,
});

var swiper2 = new Swiper(".thumbs-swiper-row", {
    spaceBetween: 16,
    autoplay: {
        delay: 3000,
        disableOnInteraction: false,
    },
    speed: 500,
    effect : "fade",
    fadeEffect: {
        crossFade: true,
    },
    thumbs: {
        swiper: swiperthumbs,
    },
    
});


var swiperthumbs = new Swiper(".thumbs-swiper-column1", {
    spaceBetween: 0,
    slidesPerView: 3,
    freeMode: true,
    direction: "vertical",
    watchSlidesProgress: true,
});

var swiper2 = new Swiper(".thumbs-swiper-column", {
    spaceBetween: 0,
    autoplay: {
        delay: 3000,
        disableOnInteraction: false,
    },
    speed: 500,
    effect : "fade",
    fadeEffect: {
        crossFade: true,
    },
    thumbs: {
        swiper: swiperthumbs,
    },
    navigation: {
        nextEl: ".swiper-button-next5",
        prevEl: ".swiper-button-prev5",
      },
    
});