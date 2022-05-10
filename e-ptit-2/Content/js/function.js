	var owl = $("#slider");
		owl.owlCarousel({
			singleItem : true,
			transitionStyle : "fade"
		});

		$('#product-slider').owlCarousel({
			singleItem : true,
			pagination : false,
			transitionStyle : "fade",
			  navigation : true,
   			 navigationText : ['<i class="fa fa-chevron-right"></i>','<i class="fa fa-chevron-left"></i>'],
		});


	$('.parallax').parallax();
	if($('#nav').length){
		$('.toggle').click(function(e) {
			$('#nav').toggleClass('active');
		});	
	}

	$( "ul.nav-icon li" ).hover(function() {
	if($(this).children('.min-shoppingcart').length){
		if($(this).children('.min-shoppingcart.remove').length){
			
		}else{
			$('.min-shoppingcart').addClass('active');
			
		}
	}
	});