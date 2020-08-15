/** @license
 * Copyright (C) 1997-2010 iNTERFACEWARE Inc.  All Rights Reserved
 *
 * Module: table.js (IGNtable)
 *
 * Description:
 *
 * A grid control developed from requirements for the iguana dashboard.
 *
 * Implemented with jQuery
 *
 * Data is fetched from the given callbacks, hence the grid is responsible
 * for requesting data from the user.
 * 
 * Author: Nasron Cheong
 * Date:   Oct 2010
 */

function IGNtable(table_id, column_headings, options, get_header_data, get_data, get_sort_array, on_visible_range, get_column_span)
{
   function _naturalCompare(iLeft, iRight)
   {
       var Left  =  iLeft.toLowerCase();
       var Right = iRight.toLowerCase();

       while( Left && Right )
	   {
	       if( /^\d/.test(Left) && /^\d/.test(Right) )
		   {
		       LeftNumber = 1* Left.match(/^\d+/);  Left  =  Left.replace(/^\d+/,'');
		       RightNumber = 1*Right.match(/^\d+/);  Right = Right.replace(/^\d+/,'');

		       if( LeftNumber != RightNumber )
			   {
			       return LeftNumber - RightNumber;
			   }
		   }
	       else
		   {
		       LeftCharacter =  Left.charCodeAt(0);  Left  =  Left.substr(1);
		       RightCharacter = Right.charCodeAt(0);  Right = Right.substr(1);

		       if( LeftCharacter != RightCharacter )
			   {
			       return LeftCharacter - RightCharacter;
			   }
		   }
	   }

       return Left < Right ? -1 : (Left == Right ? 0 : +1);
   }

   this.table_id = table_id;
   this.num_rows = 0;
   this._current_page_index = 0;

   //expects an array containing the row indexes in order
   //that they will be displayed
   this.getSortArray = (get_sort_array ? get_sort_array : function( columnInfo, columnIndex, direction ){});

   this.getColumnSpan = (get_column_span ? get_column_span : null );  
   this.onVisibleRange = (on_visible_range ? on_visible_range : function( minIndex, maxIndex ){return true;});
   this.getHeader = (get_header_data ? get_header_data : function( columnInfo, columnIndex, cell, label_span){});
   this.getData = (get_data ? get_data : function( columnInfo, columnIndex, rowIndex){});
   this._sort_array = null;
   this._last_unsorted_array = null;

   this._options = $.extend(true,
			    { sort_hover_class : 'sortable_hover',
			      sort_no_hover_class : 'sortable',
			      even_class : 'second',
			      odd_class : 'first',
			      sort_up_img : '/images/sort_up.png',
			      sort_down_img : '/images/sort_down.png',
			      default_sort_dir : 'asc',
                  min_table_width : 900,
                  rows_per_page : 0,
                  page_select : { container_id : ''}
			    },options);
   this._sort_column_name = this._options.default_sort_name;
   this._sort_direction = this._options.default_sort_dir;
   this._scroll_redrawing = false;

   this._findMinWidth = function(){
      var cols_min_width = 0;
      if (this.column_headings){
         for(col_index = 0; col_index < this.column_headings.length; col_index++) {
            var col_info = this.column_headings[col_index];
            if (!col_info.hidden){
               cols_min_width += col_info.min_width;
            }
         }
      }
      else{
         cols_min_width = this._options.min_table_width;
      }
      return cols_min_width;
   }

   this._desiredWidth = function(){
      var cols_min_width = this._findMinWidth();
      var currentWidth = $('#' + this.table_id).width();
      return (currentWidth > cols_min_width ? currentWidth : cols_min_width); 
   }

   var ThisTable = this;
   $('#'+this.table_id).append(
  '<div style="position: relative; width:100%; height:100%; overflow: hidden">'
+ '   <div style="position: absolute; top: 0px; right: 0; left: 0;" id="'+ this.table_id + '_header_div">'
+ '      <table class="grid_table" style="width:' + (this._desiredWidth()) + 'px;" id="'+ this.table_id + '_header"></table>'
+ '   </div>'
+ '   <div  style="position: absolute; top: 31px; bottom: 0; left: 0; right: 0; overflow-y: auto; overflow-x: hidden;" id="'+ this.table_id + '_body_div">'
+ '      <table class="grid_table" style="width:' + (this._desiredWidth()) + 'px;" id="'+ this.table_id + '_body"></table>'
+ '   </div>'
+ '</div>');
   
   function _scheduleRedraw( immediate, delay){
      if (ThisTable._scroll_redrawing == false) {
         ThisTable._scroll_redrawing = true;
         if (!immediate){
            window.setTimeout(function(){ThisTable.redraw()}, (delay === undefined ? 300 : 0));
         }
         else{
            ThisTable.redraw();
         }
      }
   }

   this._doResize = function(immediate, delay){
         var current_min = this._findMinWidth();
         var current_width = this._desiredWidth();
         $('#' + this.table_id + '_body').width( current_width  );
         $('#' + this.table_id + '_header').width( current_width );
         //enable/disable x scrolling only when table reaches minimum size
         var AtMinWidth = (current_width == current_min);
         $('#' + this.table_id + '_body_div').css('overflow-x', (AtMinWidth ? 'auto' : 'hidden')); 
         _scheduleRedraw(immediate, delay);
   }

   $(window).resize(function(){ ThisTable._doResize(false) })

    $('#'+this.table_id+'_body_div')
     .scroll(function(){
             $('#'+table_id+'_header_div')
             .css('margin-left','' + ( -this.scrollLeft) + 'px');
             _scheduleRedraw();
     })

   this._colsSpanList = function(row_index){
      //returns a list of the td spans for the given row
      if (this.getColumnSpan){
         var span_list = [];
         for(col_index = 0; col_index < this.column_headings.length; ) {
            var col_info = this.column_headings[col_index];
            var this_span = this.getColumnSpan(col_info, col_index, row_index);
            this_span = (this_span < 1 ? 1 : this_span);
            span_list.push(this_span);
            col_index += this_span;
         }
         return span_list;
      }
      //this makes the function O(1) when no spanning is needed
      return this._defaultSpanList;
   }

   this._colsHtml = function(row_index, span_list) {
      var col_html = '';
      for(col_index = 0; col_index < this.column_headings.length;) {
         var this_span = span_list[col_index];
         col_html += '<td id="' + this.table_id + '_cell_' + row_index + '_' + col_index + '" ' 
                   +     'class="' + this.table_id + '_cell grid_data" '
                   + 'style="' + this.column_headings[col_index].data_style + '; display:' + (this.column_headings[col_index].hidden ? 'none' : '')  + ';" '
                   + (this_span > 1 ? 'colspan="' + this_span + '" ' : ' ')
                   +'></td>';     
         col_index += this_span;
      }
      return col_html;
   }

   this.countOfPages = function(){
      if (this._options.rows_per_page > 0) {
         return Math.ceil(this.num_rows / this._options.rows_per_page);
      }
      return (this.num_rows > 0 ? 1 : 0);
   }
   
   // Returns the number of rows in the last page.
   this._lastPageCount = function(){
      var rows_per_page = this._options.rows_per_page;
      
      // #22247 - we use the formula below instead of simply
      // this.num_rows % this._options.rows_per_page
      // because we only want to return 0 if there are really
      // no rows at all.
      var mod_result = this.num_rows % rows_per_page;
      if (mod_result == 0 && this.num_rows > 0){
         // If in here, we know this.num_rows is a multiple of rows_per_page.
         return rows_per_page;
      } else {
         return mod_result;
      }
   }

   this.pageIndex = function(){
      return this._current_page_index;
   }

   this.setPageIndex = function(NewPageIndex){
      //TODO - bound check
      if (this._current_page_index != NewPageIndex) {
         this._current_page_index = NewPageIndex;
         this._createCells();
         _scheduleRedraw(true);
      }
   }

   this._numRowsCurrentPage = function() {
      if (this._options.rows_per_page > 0) {
         var last_page_count = this._lastPageCount();
         var count_of_pages = this.countOfPages();
         if (count_of_pages){
            if (this._current_page_index != count_of_pages-1) return this._options.rows_per_page;
            else return last_page_count;         
         }
      } else {
         return this.num_rows;
      }
   }

   this.rowIndexRangeForPage = function(){
      var rows_per_page = this._options.rows_per_page;
      if (rows_per_page > 0) {
         var last_page_count = this._lastPageCount();
         var count_of_pages = this.countOfPages();
         if (count_of_pages){
            var this_min = this._current_page_index * rows_per_page;
            var this_max = this_min + rows_per_page;            
            if (this._current_page_index >= count_of_pages-1) {
               this_max = this_min + last_page_count;
            }
            return {min: this_min, max: this_max};
         }
      }
      return { min: 0, max: this.num_rows};
   }

   this._createCells = function() {
      var JTable = $('#'+this.table_id + '_body');
      JTable.empty();

      var TableContentsHtml = '';
      var RowRange = this.rowIndexRangeForPage();
      for(row_index = RowRange.min; row_index < RowRange.max; row_index++) {
         var odd_even_class = (row_index % 2 ? ThisTable._options.odd_class : ThisTable._options.even_class);
         var row_html = '<tr style="height: 15px;" class="' + this.table_id + '_row '+ odd_even_class +'" id="' + this.table_id + '_row_' + row_index + '">';
         row_html += this._colsHtml(row_index, this._colsSpanList(row_index));
         row_html += '</tr>';
         TableContentsHtml += row_html;
      }

      JTable.append(TableContentsHtml);
   }

   //finding first visible
   //search for first visible, use this as max, then search before
   this._findVisible = function(SearchTop, min_index, max_index) {
      var BodyOffsets = $('#'+this.table_id + '_body_div').offset();
      var BodyHeight = $('#'+this.table_id + '_body_div').height();
      //add some margin so the check isn't so strict
      BodyOffsets.top -= 10;
      BodyHeight += 10;
      function is_visible( JRow ) {
         var RowOffset = JRow.offset();
         if (RowOffset.top >= BodyOffsets.top &&
             RowOffset.top <= (BodyOffsets.top + BodyHeight)) {
            return 0;
         }         
         else if (RowOffset.top > BodyOffsets.top){
            return 1;
         }
         else return -1;
      }

      min_index = ( min_index === undefined ? 0 : min_index);
      max_index = ( max_index === undefined ? this.num_rows : max_index);
      var found_index = -1;
      //instead of divising down the middle, put ourselves closer to the top 
      //or bottom depending on SearchTop
      var pivot_factor = (SearchTop ? (1/2) : (3/4)) 
      while(min_index < max_index) {
         var mid_index = Math.floor(pivot_factor * (max_index - min_index)) + min_index;
         var mid_visible = is_visible( $('#'+this.table_id+'_row_'+mid_index) );
         //console.log( ' mid_index ' + mid_index + ' vis ' + mid_visible  + ' in [' + min_index  + ',' + max_index + ')' );
         if (mid_visible > 0) {
            //under the window, search top half
            max_index = mid_index;
         }
         else if (mid_visible < 0) {
            //above the window search bottom half
            min_index = mid_index+1;
         }
         else { //found
            found_index = mid_index;
            //keep searching in top/bottom half
            if (SearchTop) max_index = mid_index;
            else min_index = mid_index+1;
         }
      }
      return found_index;
   }

   this.setRowCount = function(NewRowCount) {
      if (NewRowCount != this.num_rows) {
         this.num_rows = NewRowCount;
         var MaxPages = (this._options.rows_per_page ? Math.ceil(this.num_rows / this._options.rows_per_page) : 0);
         if (!MaxPages) MaxPages++;

         if (this._current_page_index >= MaxPages){
            this._current_page_index = MaxPages-1; //move to last page if over max
         }
         this._createCells();
      }
      _scheduleRedraw(true);
   }

   this._refreshColumnHeaders = function() {
      var JHeading = $('#'+this.table_id+'_header');
      var JHeadingRow = $("tr",JHeading);
      var ThisTable = this;

      //refresh column data
      var refresh_column_data = function(HeadingIndex){
          if (HeadingIndex < ThisTable.column_headings.length) {
             ThisTable.getHeader( ThisTable.column_headings[HeadingIndex], HeadingIndex, this, $('span',this).get(0) );
          }
      }
      $("th",JHeadingRow).each(refresh_column_data );
   }
   
   this.setColumnHeadings = function( new_column_headings ) {
      this.column_headings = new_column_headings;

      var JTable = $('#'+this.table_id + '_body');
      var JHeading = $('#'+this.table_id+'_header');
      var JHeadingRow = $("tr",JHeading);
      if (!JHeadingRow.size()) {
         JHeading.append('<tr class="trChannelHeadings ' + this.table_id + '_heading_row' +  '"></tr>');
         JHeadingRow = $("tr", JHeading);
      }
      var ThisTable = this;

      //recreate columns
      JHeadingRow.empty();
      for (column in this.column_headings) { 
	     function sortImg(ThisTable) { 
            return '<img src="' +
                   (ThisTable._sort_direction == 'asc' ? ThisTable._options.sort_up_img : ThisTable._options.sort_down_img) + '" ' +
                   (ThisTable._sort_direction == 'asc' ? 'style="padding: 0 0 1px 2px"' : 'style="padding: 1px 0 0 2px"' )  + '">'
               }

	     var ColumnHeading = this.column_headings[column];
	     var JHeader = $('<th class="grid_header" id="' + ThisTable.table_id +'_th_' + column + '" style="'
                         + (this.column_headings[column].hidden ? 'display:none;' : '') 
                         + (this.column_headings[column].min_width 
                            ? 'width:' + this.column_headings[column].min_width + 'px;'
                            : '') + '"><span></span></th>');
	     if (ColumnHeading.sortable) {
		 JHeader
		 .mouseover(function() { 
			 $(this).removeClass(ThisTable._options.sort_no_hover_class).addClass(ThisTable._options.sort_hover_class) 
		  })
		 .mouseout(function() { 
			 $(this).removeClass(ThisTable._options.sort_hover_class).addClass(ThisTable._options.sort_no_hover_class) 
		  })
		 .click(function(){
		     var ColumnIndex = $('th', $(this).parent()).index(this);
             var OldColumnName = ThisTable._sort_column_name;
             var OldDir = ThisTable._sort_direction;
		     ThisTable._sort_column_name = ThisTable.column_headings[ColumnIndex].name;
		     ThisTable._sort_direction = (ThisTable._sort_direction == 'asc' ? 'desc' : 'asc');
             if (ThisTable._sort_column_name != OldColumnName ||
                 ThisTable._sort_direction != OldDir){
                ThisTable._sort_array = null;
             }
  		     var JHeadings = $('#' + ThisTable.table_id + '_header');
		     $('img',JHeadings).remove();
		     $('th',JHeadings).eq(ColumnIndex).append(sortImg(ThisTable));
             _scheduleRedraw(true);
            })

		 if (ColumnHeading.name == this._options.default_sort_name) {
		     JHeader.append(sortImg(ThisTable));
		 }
         }
	     JHeadingRow.append(JHeader);
      }
      this._refreshColumnHeaders();

      //redo rows since column count changed
      JTable.empty();
      this._createCells();

      this._defaultSpanList = [];
      for (col_index in this.column_headings){
         this._defaultSpanList.push(1);
      }
   }
   this.setColumnHeadings(column_headings ? column_headings : []);
  
   this._checkSortArray = function() {
      if (this.getSortArray && this._sort_column_name) {
         
          //find column info
          var ColumnIndex = 0;
          for (ColumnIndex in this.column_headings) {
             if (this._sort_column_name == this.column_headings[ColumnIndex].name) break;
          }
          if (ColumnIndex < this.column_headings.length) {
	      var column_heading = this.column_headings[ColumnIndex];
	      var sort_array = [];
	      if (column_heading.sortable) {
             sort_array = this.getSortArray( column_heading, ColumnIndex, this._sort_direction);
	      }
	      if ( sort_array && sort_array.length ) {
		  var sort_func = column_heading.sort_function;
		  if (!sort_func) {
		      sort_func = function(a,b) {

              //TODO - this is really expensive right now, so we're doing just
              //plain string compare
              //if (typeof(a) == 'string' && typeof(b) == 'string') return _naturalCompare(a,b);

			  if (a.toLowerCase && b.toLowerCase) {
			      a = a.toLowerCase();
			      b = b.toLowerCase();
			  }
			  if (a>b) return 1;
			  else if (a<b) return -1;
			  else return 0;
		      }
		  }
		  var temp_sort = []
		  for (row_index in sort_array) {
		      temp_sort.push( { key : sort_array[row_index] , index : row_index } );
		  }
		  var sort_dir_multiplier = (this._sort_direction == 'asc' ? 1 : -1 );

          var same_as_last = false;
          if (this._sort_array && this._last_unsorted_array &&
              this._sort_array.length == sort_array.length){
             
             var temp_index = 0;
             for (; temp_index < sort_array.length; temp_index++){
                if (sort_func( sort_array[temp_index], this._last_unsorted_array[temp_index] ) != 0){
                   break;
                }
             }
             if (temp_index == sort_array.length){
                same_as_last = true;
             }
          }

          if (!same_as_last){
             this._last_unsorted_array = sort_array;
             temp_sort.sort( function(a, b){ return sort_dir_multiplier * sort_func( a.key, b.key ); } );             

             this._sort_array = [];
             for (sort_index in temp_sort) {
                this._sort_array.push( temp_sort[sort_index].index );
             }  
	      }

          }

	  }
      }
   }

   this.redraw = function() {
      var JTable = $('#'+this.table_id + '_body');
      this._refreshColumnHeaders();

      //get sort data if required
      this._checkSortArray();

      var row_range = this.rowIndexRangeForPage();

      //don't bother using visibilty if the rows per page is high enough
      var use_visibility = (this._options.rows_per_page > 50? true : false);

      var first_unspanned_row = -1;

      var count_of_get_data = 0;
      var first_visible_index = (use_visibility ? this._findVisible(true,row_range.min,row_range.max) : row_range.min);
      if (first_visible_index >= row_range.min) {
         var last_visible_index;
         if (!use_visibility){
            last_visible_index = (row_range.max ? row_range.max-1 : 0);
         }
         else {
            //do a ranged search, its likely that it will be found in fewer iterations
            var max_range = 50;
            var min_search_index = first_visible_index+1;
            var max_search_index = Math.min( first_visible_index+max_range, row_range.max);
            do{
               last_visible_index = this._findVisible(false,min_search_index,max_search_index);
               min_search_index = max_search_index;
               max_range = Math.floor(1.5 * max_range);
               max_search_index = Math.min(max_search_index + max_range, row_range.max);            
            }
            while( last_visible_index == -1 && max_search_index < row_range.max_rows );
         }

         if (last_visible_index == -1) last_visible_index = first_visible_index; 

         //notify for range
         //if false returned, no redraw is done.
         if (this.onVisibleRange(first_visible_index,last_visible_index+1)){
            //console.log('visible range : ' + first_visible_index + '->' + last_visible_index);
            for (row_index = first_visible_index; row_index <= last_visible_index; row_index++) {
               var sorted_row_index = (this._sort_array ? this._sort_array[row_index] : row_index);

               var span_list = this._colsSpanList(sorted_row_index);
               var recreate_row = false;
               for(col_index = 0; col_index < this.column_headings.length;) {
                  var this_span = span_list[col_index];
                  var cell_id = this.table_id + '_cell_'+row_index+'_'+ col_index;
                  var cell = document.getElementById(cell_id);
                  if (cell && cell.colSpan != this_span){
                     recreate_row = true;
                  }
                  col_index += this_span;
               }
               if (first_unspanned_row < 0 && span_list.length == this.column_headings.length){
                  first_unspanned_row = row_index;
               }
               if (recreate_row){
                  //span has changed for this row, recreate cells just for this row.
                  $('#' + this.table_id + '_row_' + row_index).empty().append( this._colsHtml(row_index, span_list) );
               }

               for(col_index = 0; col_index < this.column_headings.length;) {
                  var col_info = this.column_headings[col_index];
                  var this_span = span_list[col_index];
                  var cell_id = this.table_id + '_cell_'+row_index+'_'+ col_index;
                  var cell = document.getElementById(cell_id);
                  if (cell && !col_info.hidden) {
                     count_of_get_data++;
                     this.getData(col_info, col_index, sorted_row_index, cell);
                  }
                  col_index += this_span;
               }
            }
         }
      }
      
      if (first_unspanned_row < 0){
         //need to find the first row that has all cells unspanned
         //for resizing
         for (row_index = row_range.min; row_index < row_range.max; row_index++) {
            if (document.getElementById(this.table_id + '_row_'+row_index )){ //row must exist
               var sorted_row_index = (this._sort_array ? this._sort_array[row_index] : row_index);
               var span_list = this._colsSpanList(row_index);
               if (first_unspanned_row < 0 && span_list.length == this.column_headings.length){
                  first_unspanned_row = row_index;
                  break;
               }
            }
         }
      }

      //console.log('count_of_get_data : ' + count_of_get_data);
      //resize header, use first unspanned row
      var resize_headers = function() {
         if (first_unspanned_row >= 0){
            // Chrome seems to have an easier time with the measurements if we count down instead of up.
            for (var i = ThisTable.column_headings.length - 1; i > -1; i--) {
               var HeaderName = '#' + ThisTable.table_id + '_th_' + i;
               var JThisHeading = $(HeaderName);
               if (!ThisTable.column_headings[i].hidden) {
                  var SampleCellName = '#' + ThisTable.table_id + '_cell_' + first_unspanned_row  + '_' + i;
                  var TopCell = $(SampleCellName);
                  JThisHeading.width(TopCell.width());
               }
            }
         }
      }
      resize_headers();
      this._refreshPageSelect();
      this._scroll_redrawing = false;
   }   

   this.toggleColumn = function(Name, Show) {
       var JHeading = $('#'+this.table_id+'_header');  
       var JTable = $('#'+this.table_id + '_body');
       var ThisTable = this;
       for (column_index in this.column_headings) {
          if (Name == this.column_headings[column_index].name){
             this.column_headings[column_index].hidden = !Show;
             $('th:nth-child(' + (Number(column_index) + 1) + ')',JHeading).get(0).style.display = (Show ? '' : 'none');
             var row_range = this.rowIndexRangeForPage();
             for (row_index = row_range.min ;row_index < row_range.max; row_index++){
                var cell = document.getElementById(this.table_id + '_cell_' + row_index +'_' + column_index);
                if (cell){
                   cell.style.display = (Show ? '' : 'none');
                }
             }
             break;
          }
       }
       this._doResize(false,0);
   }

   this._slideInProgress = false;
   this._refreshPageSelect = function() {
      var ThisTable = this;
      if (!this._slideInProgress && this._options.page_select.container_id){
         var ContainerId = '#' + this._options.page_select.container_id;
         var SliderSelector = ContainerId + ' div:first-child';
         function makeContainers(){
            if (!$(SliderSelector).get(0)){
               $(ContainerId).append('<div></div>');
               $(ContainerId).append('<div style="float:clear; padding-top: 5px;"></div>');
            }
         }
         makeContainers();
         var Slider = $(SliderSelector).slider();
         var current_slide_val = Slider.slider('option','value');
         var current_slide_max = Slider.slider('option','max');
         var CountOfPages = this.countOfPages();

         if (this._options.rows_per_page > 0 && CountOfPages > 1) {
            //modify only if different
            //console.log('Count Of Pages ' + CountOfPages); 
            if (this.pageIndex() != current_slide_val || (current_slide_max != (CountOfPages-1))){
             $(SliderSelector)
                .slider('destroy')
                .slider(
                    {change: function(e, ui){
                         ThisTable._slideInProgress = false;
                         ThisTable.setPageIndex(ui.value);
                      },
                     slide: function(e, ui){
                          ThisTable._slideInProgress = true;
                          $(ContainerId + ' div:nth-child(2)').html('Go to page ' + (ui.value+1) + ' of ' + CountOfPages);
                       },
                     max: CountOfPages-1, 
                     value: this.pageIndex()});
              $(ContainerId +' .ui-slider').css('border','1px solid #000000');
            }
         }
         else{
            $(ContainerId).empty();
            makeContainers();
         }
         var RowRange = this.rowIndexRangeForPage();
         if (RowRange.max){
            $(ContainerId + ' div:nth-child(2)').html('Showing ' + (RowRange.min+1) + ' to ' + RowRange.max + ' of ' + this.num_rows  + ' rows');
         }
      }
   }

   //init code
   this._doResize();
}
