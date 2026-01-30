/**
* Template Name: iLanding
* Template URL: https://bootstrapmade.com/ilanding-bootstrap-landing-page-template/
* Updated: Nov 12 2024 with Bootstrap v5.3.3
* Author: BootstrapMade.com
* License: https://bootstrapmade.com/license/
*/

(function() {
  "use strict";

  /**
   * Apply .scrolled class to the body as the page is scrolled down
   */
  function toggleScrolled() {
    const selectBody = document.querySelector('body');
    const selectHeader = document.querySelector('#header');
    if (!selectHeader.classList.contains('scroll-up-sticky') && !selectHeader.classList.contains('sticky-top') && !selectHeader.classList.contains('fixed-top')) return;
    window.scrollY > 100 ? selectBody.classList.add('scrolled') : selectBody.classList.remove('scrolled');
  }

  document.addEventListener('scroll', toggleScrolled);
  window.addEventListener('load', toggleScrolled);

 

  /**
   * Scroll top button
   */
  let scrollTop = document.querySelector('.scroll-top');

  function toggleScrollTop() {
    if (scrollTop) {
      window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
    }
  }
  scrollTop.addEventListener('click', (e) => {
    e.preventDefault();
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  });

  window.addEventListener('load', toggleScrollTop);
  document.addEventListener('scroll', toggleScrollTop);

  /**
   * Animation on scroll function and init
   */
  function aosInit() {
    AOS.init({
      duration: 600,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });
  }
  window.addEventListener('load', aosInit);

  /**
   * Initiate glightbox
   */
  const glightbox = GLightbox({
    selector: '.glightbox'
  });

  /**
   * Init swiper sliders
   */
  function initSwiper() {
    document.querySelectorAll(".init-swiper").forEach(function(swiperElement) {
      let config = JSON.parse(
        swiperElement.querySelector(".swiper-config").innerHTML.trim()
      );

      if (swiperElement.classList.contains("swiper-tab")) {
        initSwiperWithCustomPagination(swiperElement, config);
      } else {
        new Swiper(swiperElement, config);
      }
    });
  }

  window.addEventListener("load", initSwiper);

  /**
   * Initiate Pure Counter
   */
  new PureCounter();

  /**
   * Frequently Asked Questions Toggle
   */
  document.querySelectorAll('.faq-item h3, .faq-item .faq-toggle').forEach((faqItem) => {
    faqItem.addEventListener('click', () => {
      faqItem.parentNode.classList.toggle('faq-active');
    });
  });

  /**
   * Correct scrolling position upon page load for URLs containing hash links.
   */
  window.addEventListener('load', function(e) {
    if (window.location.hash) {
      if (document.querySelector(window.location.hash)) {
        setTimeout(() => {
          let section = document.querySelector(window.location.hash);
          let scrollMarginTop = getComputedStyle(section).scrollMarginTop;
          window.scrollTo({
            top: section.offsetTop - parseInt(scrollMarginTop),
            behavior: 'smooth'
          });
        }, 100);
      }
    }
  });

  /**
   * Navmenu Scrollspy
   */
  let navmenulinks = document.querySelectorAll('.navmenu a');

  function navmenuScrollspy() {
    navmenulinks.forEach(navmenulink => {
      if (!navmenulink.hash) return;
      let section = document.querySelector(navmenulink.hash);
      if (!section) return;
      let position = window.scrollY + 200;
      if (position >= section.offsetTop && position <= (section.offsetTop + section.offsetHeight)) {
        document.querySelectorAll('.navmenu a.active').forEach(link => link.classList.remove('active'));
        navmenulink.classList.add('active');
      } else {
        navmenulink.classList.remove('active');
      }
    })
  }
  window.addEventListener('load', navmenuScrollspy);
  document.addEventListener('scroll', navmenuScrollspy);

})();



//  /**
//    * Mobile nav toggle
//    */
  const mobileNavToggleBtn = document.querySelector('.mobile-nav-toggle');

  function mobileNavToogle() {
    document.querySelector('body').classList.toggle('mobile-nav-active');
    mobileNavToggleBtn.classList.toggle('bi-list');
    mobileNavToggleBtn.classList.toggle('bi-x');
  }
  if (mobileNavToggleBtn) {
    mobileNavToggleBtn.addEventListener('click', mobileNavToogle);
  }

  /**
   * Hide mobile nav on same-page/hash links
   */
document.querySelectorAll('#navmenu a').forEach(link => {
  link.addEventListener('click', (e) => {

    // If link is a mega menu dropdown toggle → don't close menu
    if (link.classList.contains('mega-toggle') || link.parentElement.classList.contains('mega-dropdown')) {
      e.preventDefault(); // So that page does not refresh
      return; 
    }

    // If it's a normal nav link → close menu
    if (document.body.classList.contains('mobile-nav-active')) {
      mobileNavToogle();
    }
  });
});

//   /**
//    * Toggle mobile nav dropdowns
//    */
  document.querySelectorAll('.navmenu .toggle-dropdown').forEach(navmenu => {
    navmenu.addEventListener('click', function(e) {
      e.preventDefault();
      this.parentNode.classList.toggle('active');
      this.parentNode.nextElementSibling.classList.toggle('dropdown-active');
      e.stopImmediatePropagation();
    });
  });

// ========== DESKTOP: Hover Logic ==========
// ========== DESKTOP: Hover Logic ==========
function initDesktopMegaMenu() {
  document.querySelectorAll('.mega-left li').forEach(item => {
    item.onmouseenter = function () {
      const parent = this.closest('.mega-menu');
      const leftActive = parent.querySelector('.mega-left .active');
      const rightActive = parent.querySelector('.mega-right .mega-content.active');

      if (leftActive) leftActive.classList.remove('active');
      if (rightActive) rightActive.classList.remove('active');

      this.classList.add('active');
      const target = parent.querySelector('#' + this.dataset.target);
      if (target) target.classList.add('active');
    };
  });
}

// ========== MOBILE: Accordion Toggle ==========
function initMobileMegaMenu() {
  document.querySelectorAll('.mega-left li').forEach(li => {
    li.onclick = function (e) {
      if (window.innerWidth <= 1199) {
        e.preventDefault();

        const id = li.dataset.target;
        const content = document.getElementById(id);
        if (!content) return;

        // Move content inside li if needed
        if (!li.contains(content)) {
          li.appendChild(content);
        }

        const isOpen = li.classList.contains('open');

        // Close all others
        document.querySelectorAll('.mega-left li.open').forEach(openLi => {
          openLi.classList.remove('open');
          const openContent = openLi.querySelector('.mega-content');
          if (openContent) openContent.style.display = 'none';
        });

        // Toggle current
        if (!isOpen) {
          li.classList.add('open');
          content.style.display = 'block';
        } else {
          li.classList.remove('open');
          content.style.display = 'none';
        }
      }
    };
  });
}

// ========== Toggle Main Mega Menu ==========
document.querySelectorAll('.mega-toggle').forEach(toggle => {
  toggle.addEventListener('click', function (e) {
    if (window.innerWidth <= 1199) {
      e.preventDefault();
      this.parentElement.classList.toggle('open');
    }
  });
});

// ========== Handle Resize ==========
function handleResize() {
  const isDesktop = window.innerWidth > 1199;

  document.querySelectorAll('.mega-menu').forEach(menu => {
    const leftItems = menu.querySelectorAll('.mega-left li');

    leftItems.forEach(li => {
      const id = li.dataset.target;
      const content = document.getElementById(id);
      if (!content) return;

      if (isDesktop) {
        // Move content back to .mega-right (for desktop)
        const rightPanel = menu.querySelector('.mega-right');
        if (rightPanel && !rightPanel.contains(content)) {
          rightPanel.appendChild(content);
        }
        li.classList.remove('open');
        content.style.display = '';
      } else {
        // Move content inside <li> (for mobile)
        if (!li.contains(content)) {
          li.appendChild(content);
        }
        li.classList.remove('active');
        content.style.display = 'none';
      }
    });
  });

  // Reinitialize correct events each time
  if (isDesktop) {
    initDesktopMegaMenu();
  } else {
    initMobileMegaMenu();
  }
}

window.addEventListener('load', handleResize);
window.addEventListener('resize', handleResize);
