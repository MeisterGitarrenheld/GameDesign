if (typeof jQuery !== 'undefined') {
    (function($) {
        $(document).ajaxStart(function() {
            showLoadingSpinner();
        }).ajaxStop(function() {
            hideLoadingSpinner();
        });
    })(jQuery);
}

/**
* Searches all navigation bar links and activates them, if the corresponding
* section is in the view port, else deactivates them.
*/
var updateNavigation = function() {
  var currentPos = $(window).scrollTop();

  $('.nav-link').each(function() {
    var sectionLink = $(this);
    
    // capture the height of the navbar
	var navWrapper = $('#nav-wrapper');
    var navHeight = navWrapper.outerHeight();
	var sectionId = sectionLink.attr('href');
	var isHomeLink = sectionId.includes('home');
	
	var section = $(sectionId);
	var sectionPosTop = section.position().top;
	var sectionHeight = section.height();

	// subtract the navbar height from the top of the section
	var activeClass = 'active';
	if((currentPos === 0 && isHomeLink) || (currentPos !== 0 && (sectionPosTop - navHeight  < currentPos && sectionHeight + sectionPosTop - navHeight > currentPos))) {
	  $('.nav li').removeClass(activeClass);
	  sectionLink.addClass(activeClass);
	} else {
	  sectionLink.removeClass(activeClass);
	}

	var navBgClass = 'bg-dark';
	if(currentPos <= 100){
        navWrapper.removeClass(navBgClass);
        navWrapper.addClass('bg-transparent');
	}
	else{
        navWrapper.removeClass('bg-transparent');
        navWrapper.addClass(navBgClass);
	}
  })
};

// activate the navigation buttons on load
$(window).on('load', updateNavigation);

// update the navigation buttons active state on scroll
$(window).on("scroll", updateNavigation);

/**
* Provide smooth scrolling on page link clicks.
*/
$('a[href*="#"]').click(function() {
    if (location.pathname.replace(/^\//,'') === this.pathname.replace(/^\//,'')
        || location.hostname === this.hostname) {
		
        var target = $(this.hash);
		var currentPos = $(window).scrollTop();
		var scrollPos = 0;

        if (target.length) {
			var navHeight = $('#nav-wrapper').outerHeight();
            scrollPos = target.offset().top - navHeight + 1;
        }
		
		var diff = Math.abs(currentPos - scrollPos);
		var minValue = Math.max(diff, 200);
		var duration = Math.min(minValue, 1000);
		
		var alreadyDone = scrollPos <= 0 && currentPos == 0 || diff <= 2;
		
		if(!alreadyDone){
			showLoadingSpinner();
			$('html,body').animate({
				 scrollTop: scrollPos
			}, duration, hideLoadingSpinner);
		}
		return false;
    }
});

var loadHomePage = function(){
	window.location='#home';
}

function showLoadingSpinner(){
    $('.modal').modal('show');
}

function hideLoadingSpinner(){
    $('.modal').modal('hide');
}

/*
function adjustHeaderBackground(){
	var header = $('#home');
	var headerHeight = header.height();
	var headerWidth = header.width();
	
	var containClass = 'header-bg-contain';
	var coverClass = 'header-bg-cover';
	if(headerHeight * 1.5 < headerWidth || headerWidth > 3500){
		// white border with contain, so we have to stretch with cover
		header.removeClass(containClass);
		header.addClass(coverClass);
		console.log('cover: ');
	}
	else {
		header.removeClass(coverClass);
		header.addClass(containClass);
		console.log('contain');
	}
	
	console.log('headerHeight: ' + headerHeight);
	console.log('headerHeight * 1.5: ' + (headerHeight * 1.5));
	console.log('headerWidth: ' + headerWidth);
}

$(window).on('load', adjustHeaderBackground);

new ResizeSensor($('#home'), adjustHeaderBackground);
*/
