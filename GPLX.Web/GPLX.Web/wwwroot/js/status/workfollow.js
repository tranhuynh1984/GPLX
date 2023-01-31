var WorkFollow = function() {
    let base = this;
    base.navigationContainer = $('#cd-nav');
    base.mainNavigation = base.navigationContainer.find('#cd-main-nav ul');
    base.setup = function() {
        $(window).scroll(base.scroller);
        $('.cd-nav-trigger').on('click', function () {
            $(this).toggleClass('menu-is-open');
            //we need to remove the transitionEnd event handler (we add it when scolling up with the menu open)
            base.mainNavigation.off('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend').toggleClass('is-visible');
        });
    }
    base.scroller = function () {
        var offset = 300;
        if ($(window).scrollTop() > offset && !base.navigationContainer.hasClass('is-fixed')) {
            base.navigationContainer.addClass('is-fixed').find('.cd-nav-trigger').one('webkitAnimationEnd oanimationend msAnimationEnd animationend', function () {
                base. mainNavigation.addClass('has-transitions');
            });
        } else if ($(window).scrollTop() <= offset) {
            //check if the menu is open when scrolling up
            if (base.mainNavigation.hasClass('is-visible') && !$('html').hasClass('no-csstransitions')) {
                //close the menu with animation
                base.mainNavigation.addClass('is-hidden').one('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend', function () {
                    //wait for the menu to be closed and do the rest
                    base.mainNavigation.removeClass('is-visible is-hidden has-transitions');
                    base.navigationContainer.removeClass('is-fixed');
                    $('.cd-nav-trigger').removeClass('menu-is-open');
                });
                //check if the menu is open when scrolling up - fallback if transitions are not supported
            } else if (base.mainNavigation.hasClass('is-visible') && $('html').hasClass('no-csstransitions')) {
                base.mainNavigation.removeClass('is-visible has-transitions');
                base.navigationContainer.removeClass('is-fixed');
                $('.cd-nav-trigger').removeClass('menu-is-open');
                //scrolling up with menu closed
            } else {
                base.navigationContainer.removeClass('is-fixed');
                base.mainNavigation.removeClass('has-transitions');
            }
        }
    }
}

$(document).ready(function() {
    var wl = new WorkFollow();
    wl.setup();
});