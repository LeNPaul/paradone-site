/** @license
 * Copyright (c) 1997-2011 iNTERFACEWARE Inc.  All rights reserved.
**/

 /*
 * Module: slider.js (SLIDEcontrol)
 *
 * Description:
 *
 * A javascript class that fills a given element with a control,
 * which can transition to new contents using a slide animation.
 *
 * Implemented with jQuery
 *
 * User should construct a new control, passing in an id of an empty div
 * tag, and setting initial contents of the visible slide.
 *
 * Calling next/previous with new content will transition right/left
 * to the new content with a slide animation.
 *
 * Almost all calls are queued through jQuery's animation queue
 * to ensure that we dont run into weird state situations when methods
 * are called in quick succession.
 * 
 * Author: Nasron Cheong
 * Date:   July 5 2009
 */

function SLIDEcontrol(container_id, options)
{
   this.resizeFrames = function()
   {
      var slider = this;
      $('#'+slider.container_id+'_frame_holder').queue(
      function()
      { 
         var new_height =    $('#'+slider.container_id).innerHeight();
         f_holder = $('#'+slider.container_id+'_frame_holder');
         f_holder.height( 3*new_height );
         f_holder.children().height( new_height );
         f_holder.css('margin-top','-'+new_height+'px' );
         $('#'+slider.container_id+'_frame_view_port').height(new_height);
         slider.current_height = new_height;
         $(this).dequeue();
      });
   }

   this.next = function(Content)
   {
      var slider = this;
      var j_holder = $('#'+slider.container_id+'_frame_holder');
   
      j_holder.queue( function(){
         $('#'+slider.container_id+'_frame_holder > :last').html(Content);
         $(this).dequeue();
      });
      j_holder.animate( 
         {marginTop: '-='+slider.current_height}, 
         slider.anim_duration);
      j_holder.queue(function(){
         //remove left frame and add right frame
         $('#'+slider.container_id+'_frame_holder > :first-child').remove();
         $('#'+slider.container_id+'_frame_holder').css('margin-top','-'+slider.current_height+'px');
         slider.appendNextFrame('&nbsp');
         $(this).dequeue();
      });
   }

   this.previous = function(Content)
   {
      var slider = this;
      var j_holder = $('#'+slider.container_id+'_frame_holder');
      j_holder.queue( function(){
        $('#'+slider.container_id+'_frame_holder > :first-child').html(Content);
        $(this).dequeue();
      });
      j_holder.animate( 
         {marginTop: '+='+this.current_height}, 
         this.anim_duration);
      j_holder.queue(function(){
        //remove right frame and add a left frame
        $('#'+slider.container_id+'_frame_holder > :last-child').remove();
        $('#'+slider.container_id+'_frame_holder').css('margin-top','-'+slider.current_height+'px');
        slider.appendPreviousFrame('&nbsp');
        $(this).dequeue();
        // Silly hack to fix IE8 bug which makes the area disappear after sliding
        // (the area would later reappear after moving the mouse enough).
        this.style.display = 'none';
        this.style.display = '';
      });
   }

   this.setContent = function(Content)
   {
      var slider = this;
      $('#'+this.container_id+'_frame_holder').queue(function(){
         $('#'+slider.container_id+'_frame_holder > :first').next().html(Content);
         $(this).dequeue();
      });
   }

   var frameHtml = function(Content,Height)
   {
      var Out = '<div style="height:' + Height + 'px;">' + Content + '</div>'
      return Out;
   }

   this.appendNextFrame = function(Content)
   {
       $('#'+this.container_id+'_frame_holder').append(frameHtml(Content,this.current_height));
   }

   this.appendPreviousFrame = function(Content)
   {
      $('#'+this.container_id+'_frame_holder').prepend(frameHtml(Content,this.current_height));
   }

   this.modifyContents = function( doModify )
   {
      var slider = this;
      $('#'+this.container_id+'_frame_holder').queue(function(){
        doModify($('#'+slider.container_id+'_frame_holder > :first').next().get(0));
        $(this).dequeue();
      });      
   }
   
   //Constructor Start
   if (!options)
   {
      options = 
      {
         anim_duration: 1000,
         content : 'fill me'
      }
   }

   this.current_height = $('#'+this.container_id).innerHeight();
   this.anim_duration = options.anim_duration;
   this.container_id =  container_id;
   $('#'+this.container_id).html(
      '<div id="'
         + this.container_id 
         + '_frame_view_port" style="height:100%;'
         + 'overflow-y:hidden;">'
         + '   <div id="' + this.container_id + '_frame_holder"></div>'
         + '</div>'
   );

   this.appendNextFrame('&nbsp;');
   this.appendNextFrame(options.content);
   this.appendNextFrame('&nbsp;');
   this.resizeFrames();
}
