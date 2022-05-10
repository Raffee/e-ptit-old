
(function( $, undefined ) {
    
    $.widget("ryanf.crosswordwidget", $.ui.mouse, {

            options : {
                wordObjlist : null,
                gridWidth : 10,
                gridHeight : 12,
                otherCells: null,
                isSeparateHints: false
            },
			_mapEventToCell: function(event) {
                var currentColumn = Math.ceil((event.pageX - this._cellX) / this._cellWidth);
                var currentRow = Math.ceil((event.pageY - this._cellY) / this._cellHeight);
                var el = $('#rf-tablegrid tr:nth-child('+currentRow+') td:nth-child('+currentColumn+')');
                return el;
			},
            
            _create : function () {
                //member variables
                this.model = GameWidgetHelper.prepGrid(this.options.gridWidth, this.options.gridHeight, this.options.wordObjlist, this.options.otherCells, this.options.isSeparateHints)
                this.startedAt  = new Root();
                this.hotzone    = new Hotzone();
                this.arms       = new Arms();
				
                
				GameWidgetHelper.renderGame(this.element[0],this.model);
				
				this.options.distance=0; // set mouse option property
                this._mouseInit();
                
                var cell = $('#rf-tablegrid tr:first td:first');
        this._cellWidth = cell.outerWidth();
        this._cellHeight = cell.outerHeight();
        this._cellX = cell.offset().left;
        this._cellY = cell.offset().top;
            },//_create
            
            destroy : function () {
                
                this.hotzone.clean();
                this.arms.clean();
                this.startedAt.clean();
                
				this._mouseDestroy();
				return this;
                
            },
            
            //mouse callbacks
            _mouseStart: function(event) {
				var panel = $(event.target).parents("div").attr("id");
				if ( panel == 'rf-wordcontainer') {
					//User has requested help. Identify the word on the grid
					//We have a reference to the td in the cells that make up this word
					var idx = $(event.target).parent().children().index(event.target);

					//var selectedWord = this.model.wordList.get(idx);
					//$(selectedWord.cellsUsed).each ( function () {
					//	debugger;
					//	var txt = $("#"+this.txtId);
					//	if(txt.length > 0)
					//	{debugger;
					//	$(this.td).html("<td class='td-basic'>" + this.value + "</td>");
					//}
					//});
					
				}

            }
            
        }
    ); //widget


$.extend($.ryanf.crosswordwidget, {
	version: "0.0.1"
});

//------------------------------------------------------------------------------
// VIEW OBJECTS 
//------------------------------------------------------------------------------
/*
 * The Arms represent the cells that are selectable once the hotzone has been 
 * exited/passed
 */
function Arms() {
    this.arms = null;

    //deduces the arm based on the cell from which it exited the hotzone.
    this.deduceArm = function (root, idx) {

        this.returnToNormal(); //clear old arm
        var ix = $(root).parent().children().index(root);

        //create the new nominees
        this.arms = new Array();

        //create surrounding nominees
        switch (idx) {
            case 0: //horizontal left
                this.arms = $(root).prevAll();
                break;

            case 1: //horizontal right
                this.arms = $(root).nextAll();
                break;

            case 2: //vertical top
                var $n = this.arms;
                $(root).parent().prevAll().each( function() {
                    $n.push($(this).children().get(ix));
                });
                
                break;

            case 3: //vertical bottom
                var $o = this.arms;
                $(root).parent().nextAll().each( function() {
                    $o.push($(this).children().get(ix));
                });
                break;

            case 4: //right diagonal up
                
                var $p = this.arms;

                //for all prevAll rows
                var currix = ix;
                $(root).parent().prevAll().each( function () {
                    $p.push($(this).children().get(++currix));
                });
                break;

            case 5: //left diagonal up
                var $q = this.arms;

                //for all prevAll rows
                var currixq = ix;
                $(root).parent().prevAll().each( function () {
                    $q.push($(this).children().get(--currixq));
                });
                break;

            case 6 : //left diagonal down
                var $r = this.arms;
                //for all nextAll rows
                var currixr = ix;
                $(root).parent().nextAll().each( function () {
                    $r.push($(this).children().get(++currixr));
                });
                break;

            case 7: //right diagonal down
                var $s = this.arms;
                //for all nextAll rows
                var currixs = ix;
                $(root).parent().nextAll().each( function () {
                    $s.push($(this).children().get(--currixs));
                });
                break;


        }
        for (var x=1;x<this.arms.length;x++) {
            Visualizer.arm(this.arms[x]);
        }
    }

	//lights up the cells that from the root cell tothe current one
    this.glowTo = function (upto) {
        var to = $(this.arms).index(upto);
        
        for (var x=1;x<this.arms.length;x++) {
            
            if (x<=to) {
                Visualizer.glow(this.arms[x]);
            }
            else {
                Visualizer.arm(this.arms[x]);

            }
        }
    }
	
	//clear out the arms 
    this.returnToNormal = function () {
        if (!this.arms) return;
        debugger;
        for (var t=1;t<this.arms.length;t++) { //don't clear the hotzone
            Visualizer.restore(this.arms[t]);
        }
    }
    
    
    this.clean = function() {
        $(this.arms).each(function () {
           Visualizer.clean(this); 
        });
    }
 
}

/*
 * Object that represents the cells that are selectable around the root cell
 */
function Hotzone() {
    
    this.elems = null;
    
    //define the hotzone
    //select all neighboring cells as nominees
    this.createZone = function (root) {
        this.elems = new Array(); 
        
        var $tgt = $(root);
        var ix = $tgt.parent().children().index($tgt);

        var above = $tgt.parent().prev().children().get(ix); // above
        var below = $tgt.parent().next().children().get(ix); // below

        //nominatedCells.push(event.target); // self
        this.elems.push($tgt.prev()[0],$tgt.next()[0]); //horizontal
        this.elems.push( above, below, 
                            $(above).next()[0],$(above).prev()[0], //diagonal
                            $(below).next()[0],$(below).prev()[0] //diagonal
                          );


        $(this.elems).each( function () {
            if ($(this)!=null) {
                Visualizer.arm(this);
            }
        });
        
    }
    //give the hotzone some intelligence
    this.index = function (elm) {
        return $(this.elems).index(elm);
    }

    this.setChosen = function (chosenOne) {
        for (var x=0;x<this.elems.length;x++) {
            Visualizer.arm(this.elems[x]);
        }
        if (chosenOne != -1) {
            Visualizer.glow(this.elems[chosenOne]);
        }

    }

    this.returnToNormal = function () {

        for (var t=0;t<this.elems.length;t++) {
            Visualizer.restore(this.elems[t]);
        }
    }
    
    this.clean = function() {
        $(this.elems).each(function () {
           Visualizer.clean(this); 
        });
    }
    
}

/*
 * Object that represents the first cell clicked
 */
function Root() {
    this.root = null;
    
    this.setRoot = function (root) {
        this.root = root;
        Visualizer.glow(this.root);
    }
    
    this.returnToNormal = function () {
        Visualizer.restore(this.root);
    }
    
    this.isSameCell = function (t) {
        return $(this.root).is($(t));
    }
    
    this.clean = function () {
        Visualizer.clean(this.root);
    }
    
}

/*
 * A utility object that manipulates the cell display based on the methods called.
 */
var Visualizer = {
	
    glow : function (c) {
        $(c).removeClass("rf-armed")
            .removeClass("rf-selected")
            .addClass("rf-glowing");
    },
    
    arm : function (c) {
        $(c)//.removeClass("rf-selected")
            .removeClass("rf-glowing")
            .addClass("rf-armed");
        
    },
    
    restore : function (c) {
        $(c).removeClass("rf-armed")
            .removeClass("rf-glowing");
            
        if ( c!=null && $.data(c,"selected") == "true" ) {
            $(c).addClass("rf-selected");
        }
    },
    
    select : function (c) {
        $(c).removeClass("rf-armed")
            .removeClass("rf-glowing")
			.animate({'opacity' : '20'}, 500, "linear", function () {
				$(c).removeClass("rf-highlight").addClass("rf-selected")
				.animate({'opacity' : 'show'}, 500, "linear")
			})
            
        
    },
    
    highlight : function (c) {
        $(c).removeClass("rf-armed")
            .removeClass("rf-selected")
			.addClass("rf-highlight");
    },
	
    signalWordFound : function (w) {

		$(w).animate({"opacity": 'hide'},1000,"linear",
					 function () {
						 $(w).addClass('rf-foundword').animate({"opacity": 'show'},1000,"linear")
					 });
    },

	


	clean : function (c) {
        $(c).removeClass("rf-armed")
            .removeClass("rf-glowing")
            .removeClass("rf-selected");
            
        $.removeData($(c),"selected");    
        
    }
}

//--------------------------------------------------------
// OBJECTS RELATED TO THE MODEL
//------------------------------------------------------------------------------

/*
 * Represents the individual cell on the grid
 */
function Cell() {
    this.DEFAULT = "-";
    this.isHighlighted = false;
    this.isHint = false;
    this.isMultiLetter = false;
    this.value = this.DEFAULT;
    this.parentGrid = null;
    this.isUnwritten = function () {
        return (this.value == this.DEFAULT);
    };
    this.isSelected = false;
    this.isSelecting = true;
	this.td = null; // reference to UI component
	this.word = null;
	this.wordsList = [];
    
}//Cell

/*
 * Represents the Grid
 */
function Grid() {
    this.cells = null;
    this.otherCells = null;
    
    this.directions = ["Horizontal", "Vertical"];
    
    this.initializeGrid = function(width, height, otherCells) {
        this.otherCells = otherCells;
        this.cells = new Array(height);
        for (var i=0;i<height;i++) {
            this.cells[i] = new Array(width);
            for (var j=0;j<width;j++) {
                var c = new Cell();
                c.parentgrid = this;
                this.cells[i][j] = c;
            }
        }
    }
    
    
    this.getCell = function(row,col) {
        return this.cells[row][col];
    }
    
    this.createHotZone = function(uic) {
        var $tgt = uic;

        var hzCells = new Array(); 
        var ix = $tgt.parent().children().index($tgt);

    }
    
    this.size1 = function() {
        return this.cells.length;
    }
    
    this.height = function(){
        return this.cells.length;
    }
    
    this.width = function(){
        return this.cells[0].length;
    }
    
    //place word on grid at suggested location
    this.put = function(row, col, word, direction) {
		//word = new Word(word);
        //Pick the right Strategy to place the word on the grid
        var populator = eval("new "+ eval("this.directions["+direction+"]") +"Populator(row,col,word, this)");
        var isPlaced= populator.populate();
        
        //Didn't get placed.. brute force-fit (if possible)
        if (!isPlaced) {
            for (var x=0;x<this.directions.length;x++) {
                var populator2 = eval("new "+ eval("this.directions["+x+"]") +"Populator(row,col,word, this)");
                var isPlaced2= populator2.populate();
                if (isPlaced2) break;                
            }            
        }
    }
    
    this.fillGrid = function () {
        debugger;
		var letterIndex = 0;
		for(var i=0; i<this.otherCells.length; i++)
		{
		    //debugger;
		    var myCellObj = this.otherCells[i];
		    if (myCellObj.type == 1) {
		        this.cells[myCellObj.row][myCellObj.col].isHint = true;
		    }
		    else if (myCellObj.type == 3) {
		        debugger;
		        this.cells[myCellObj.row][myCellObj.col].isMultiLetter = true;
		        this.cells[myCellObj.row][myCellObj.col].value = myCellObj.css;
		    }
            this.cells[myCellObj.row][myCellObj.col].css = myCellObj.css;
		}        
    }
}//Grid

/*
 * Set of strategies to populate the grid.
 */
//Create a Horizontal Populator Strategy 
function HorizontalPopulator(row, col, word, grid) {
    
    this.grid = grid;
    this.row =  row;
    this.col = col;
    this.word = word;
    this.width = this.grid.width();
    this.height = this.grid.height();
    this.cells = this.grid.cells;
    
    //populate the word
    this.populate = function() {
        // try and place word in this row

        // check if this row has a contigous block free
        // 1. starting at col (honour the input)
        this.writeWord();
        return (word.isPlaced);        
        
    }//populate
    
    //write word on grid at given location
    //also remember which cells were used for displaying the word
    this.writeWord = function () {

        var chars = word.chars;

        if (word.hint != null && word.hint.indexOf("|") > -1) {
            chars = word.hint.split("|");
            word.originalValue = word.originalValue.replace("|", "").replace("|", "");
        }

        for (var i = 0; i < chars.length; i++) {
            var c = new Cell();
            if (this.cells[this.row][this.col + i])
                c = this.cells[this.row][this.col + i];
            c.value = chars[i];
            c.word = word;
            c.wordsList.push(word);
			c.txtId = "dataR" + this.row.toString() + "C" + (this.col+i).toString();
            this.cells[this.row][this.col+i] = c;
            word.containedIn(c);
            word.isPlaced = true;
        }

    }
       
}//HorizontalPopulator

//Create a Vertical Populator Strategy 
function VerticalPopulator(row, col, word, grid) {
    
    this.grid = grid;
    this.row =  row;
    this.col = col;
    this.word = word;
    this.height = this.grid.height();
    this.width = this.grid.width();
    this.cells = this.grid.cells;
    
    //populate the word
    this.populate = function() {
        // try and place word in this row

        // check if this row has a contigous block free
        // 1. starting at col (honour the input)
        debugger;
        this.writeWord();
        return (word.isPlaced);            
        
    }//populate
    
    //write word on grid at given location
    this.writeWord = function () {
        var chars = word.chars;
        if (word.hint != null && word.hint.indexOf("|") > -1) {
            chars = word.hint.split("|");
        }

            debugger;
        for (var i = 0; i < chars.length; i++) {
            var c = new Cell();
            if (this.cells[this.row + i][this.col])
                c = this.cells[this.row + i][this.col];
            c.value = chars[i];
            c.word = word;
            c.wordsList.push(word);
			c.txtId = "dataR" + (this.row+i).toString() + "C" + (this.col).toString();
            this.cells[this.row+i][this.col] = c; //CHANGED
            word.containedIn(c);
            word.isPlaced = true;
        }        
    }
  
}//VerticalPopulator

/*
 * Container for the Entire Model
 */
function Model() {
    this.grid= null;
    this.wordList= null;
    
    this.init = function (grid, list, words, isSeparateHints) {
        this.grid = grid;
        this.wordObjectList = list;
        this.wordList = words;
        this.isSeparateHints = isSeparateHints;
		
        for (var i=0;i<this.wordObjectList.length;i++) {
            grid.put( this.wordObjectList[i].row, this.wordObjectList[i].col, this.wordList.get(i), this.wordObjectList[i].dir );
        }

    }
    
}//Model

/*
 * Represents a word on the grid
 */
function Word(val, hint, dir) {
    this.value = val;//.toLowerCase();
    this.originalValue = this.value;
    this.originalCaseValue = val;
    this.isFound= false;
    this.cellsUsed = new Array();
    this.hint = hint;
    this.dir = dir;

    this.isPlaced = false;
    this.row = -1;
    this.col = -1;
    this.size = -1;
    this.chars = null;

    this.init = function () {
        this.chars = this.value.split("");
        this.size = this.chars.length;
    }
    this.init();
    
    this.containedIn = function (cell) {
        this.cellsUsed.push(cell);
    }
	
	this.reverse = function reverse() {
		return this.value.split('').reverse().join('');
	}
    
    this.checkIfSimilar = function (w) {
        if (this.originalValue == w || this.value == w) {
            this.isFound = true;
            return true;
        }
        return false;
    }
    

}

/*
 * Represents the list of words to display
 */
function WordList() {
    this.words = new Array();
    
    this.loadWords = function (csvwords) {
        var $n = this.words;
        $(csvwords.split(",")).each(function () {
            $n.push(new Word(this));
        });
        
    }
	
	this.loadWordsFromArray = function(wordsObjArray){
		var $n = this.words;
		for (var i = 0; i < wordsObjArray.length; i++)
		    $n.push(new Word(wordsObjArray[i].word, wordsObjArray[i].hint, wordsObjArray[i].dir));
	}
    
    this.add = function(word) {
        //here's where we reverse the letters randomly
        if (Math.random()*10 >5) {
            var s="";
            for (var i=word.size-1;i>=0;i--) {
                s = s+ word.value.charAt(i);
            }
            word.value = s;
            word.init();
        }
        this.words[this.words.length] = word;
    }
    
    this.size = function() {
        return this.words.length;
    }
    
    this.get = function(index) {
        return this.words[index];
    }
    
    this.isWordPresent = function(word2check) {
        for (var x=0;x<this.words.length;x++) {
            if (this.words[x].checkIfSimilar(word2check) || this.words[x].checkIfSimilar(word2check.split('').reverse().join(''))) return x;
        }
        return -1;
    }
}

/*
 * Utility class
 */
var Util = {
    random : function(max) {
        return Math.floor(Math.random()*max);
    },
    
    log : function (msg) {
        $("#logger").append(msg);
    }
} 


//------------------------------------------------------------------------------
// OBJECTS RELATED TO THE CONTROLLER
//------------------------------------------------------------------------------
/*
 * Main controller that interacts with the Models and View Helpers to render and
 * control the game
 */
var GameWidgetHelper = {
    prepGrid: function (width, height, wordObjectList, otherCells, isSeparateHints) {
		var grid = new Grid();
		grid.initializeGrid(width, height, otherCells);

		var wordList = new WordList();
		wordList.loadWordsFromArray(wordObjectList);

		var model = new Model();
		model.init(grid, wordObjectList, wordList, isSeparateHints);
		grid.fillGrid();
		return model;

	},
	
    renderGame : function(container, model) {
        var grid = model.grid;
        var cells = grid.cells;
        
        debugger;
        var puzzleGrid = "<div id='rf-searchgamecontainer'><table id='rf-tablegrid' cellspacing=0 cellpadding=0 class='rf-tablestyle crossword'>";
        for (var i=0;i<grid.height();i++) {
            puzzleGrid += "<tr>";
            for (var j=0;j<grid.width();j++) {
				if(cells[i][j].isHint == true)
				    puzzleGrid += "<td class='td-basic " + cells[i][j].css + "'>" + cells[i][j].value + "</td>";
				else
				{
					if(cells[i][j].word)
					{
					    var wordValue = "";
					    if (cells[i][j].wordsList.length > 0) {
					        for (var x = 0; x < cells[i][j].wordsList.length; x++) {
					            wordValue += cells[i][j].wordsList[x].value + " ";
					        }
					    }

					    puzzleGrid += '<td class="' + cells[i][j].css + '"><input type="text" data-letter-id="' + cells[i][j].value + '"  onfocus="VirtualKeyboard.attachInput(this)" onblur="validateLetter(this)"';
						puzzleGrid += ' id="dataR' + i.toString() + "C" + j.toString() + '" data-is-solved="false" data-word="' + wordValue + '" class="txt-data"></td>';
					}
					else{
					    puzzleGrid += "<td class='td-basic " + cells[i][j].css + " invisi'>&nbsp;</td>";
					}
				}
            }
            puzzleGrid += "</tr>";
        }
        puzzleGrid += "</table></div>";
        $(container).append(puzzleGrid);

        var x=0;
        var y=0;
        $("tr","#rf-tablegrid").each(function () {
            $("td", this).each(function (col){
                var c = cells[x][y++];
				$.data(this,"cell",c);
				c.td = this;
            }) 
            y=0;
            x++;
        });


    },
	
	signalWordFound : function(idx) {
		var w = $("li").get(idx);
		Visualizer.signalWordFound(w);
	}
	
}


})(jQuery);
