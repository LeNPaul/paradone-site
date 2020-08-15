/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript routines for edit_channel_group.cs page

// Determines the channel list to show in the membership control.
function filterTextChanged(Side)
{
   var Filter = $("#filter_" + Side);
   var ChannelSelector = $("#channels_" + Side)[0];
   var ChannelCounter = $("#element_count_" + Side)[0];

   var ChannelStringBuilder = [];
   var Options = ChannelSelector.getElementsByTagName("DIV");

   for(var OptionsIndex = Options.length -1; OptionsIndex >= 0 ; --OptionsIndex)
   {
      ChannelStringBuilder.push(Options[OptionsIndex].id);
      ChannelStringBuilder.push(',');
   }

   //Drop the last comma.
   ChannelStringBuilder[ChannelStringBuilder.length -1] = "";
   filterChannels(Side, ChannelStringBuilder.join(''), Filter.val(), ChannelSelector, ChannelCounter);

   var ClearIcon = $("#clear_filter_icon_" + Side);
   
   if(Filter.val() == "")
   {
      ClearIcon.css("visibility", "hidden");
   }
   else
   {
      ClearIcon.css("visibility", "visible");
   }
}

function moveSelected(FromFix, ToFix, IsMoveAll)
{
   var From = $("#channels_" + FromFix)[0];
  
   var FromOptions = From.getElementsByTagName("DIV");
   for(var OptionsIndex = FromOptions.length -1; OptionsIndex >= 0 ; --OptionsIndex)
   {
      var Class = FromOptions[OptionsIndex].attributes.getNamedItem("class");	   
      if(IsMoveAll || ( Class && Class.value == "MSCselected"))
      {
         if(FromOptions[OptionsIndex].style.display != "none")
	 { 
            MSCmove("CGedit" + $("#old_group_name").val(), {"element":FromOptions[OptionsIndex]}, ToFix);
	 }
      }
   }
}

function validateGroupName(Name)
{
   var EncodedName = encodeURIComponent(Name);

   var AJAXCommandName = "validate_channel_group_name";
   var AJAXVariableName = "Name=";

   var Result = true;

   $.ajax({url: AJAXCommandName, data: AJAXVariableName + EncodedName,
      async: false, 
      success: 
      function(data) 
      {                        
         try
         {
            var Response = data;
 
            if(Response) 
            {
               Result = Response.Result;

               if(!Result)
               {
                  $("#result").attr('class', 'error');
                  $("#result_title").attr('class', 'error').text('Error');
                  $("#result_content").text(Response.ErrorMessage);
                  $("#result").slideDown("fast");
               }
            }
         }
         catch(err)
         {                             
         }        
      },

      error:
      function()
      {           
         MiniLogin.show('Iguana is not Responding', function() { validateGroupName(Name) });
      }
   });

   return Result;
}

function update_group()
{  
   if("" == $("#group").val())
   {
      $("#result").attr('class', 'error');
      $("#result_title").attr('class', 'error').text('Error');
      $("#result_content").text("Channel groups require a name.");
      $("#result").slideDown("fast");

      document.channel_group_data.group.focus();
      return false;
   }
   else
   {
      if("createGroup" == $("#action").val())
      {
         var Result = validateGroupName($("#group").val());

         if(!Result)
         {
            return false;
         }
      }
      else 
      {
         if($("#group").val() != $("#old_group_name").val())
         {
            var Result = validateGroupName($("#group").val());

            if(!Result)
            {
               return false;
            }
         }
      } 
   }

   if("All Channels" != $("#old_group_name").val())
   {
      var ChannelsSelector = $("#channels_right")[0];
 
      var ChannelsStringBuilder = [];
      var Options = ChannelsSelector.getElementsByTagName("DIV");
      for ( var OptionsIndex = Options.length -1; OptionsIndex >= 0 ; --OptionsIndex )
      {
         // Change to values (guid) later.
         ChannelsStringBuilder.push(Options[OptionsIndex].id);
         ChannelsStringBuilder.push(',');
      }

      ChannelsStringBuilder[ChannelsStringBuilder.length -1] = "";
   
      $("#included_channels").val(ChannelsStringBuilder.join(''));
   }

   return true;
}

var CGEmode = null;

$(document).ready(function() 
{
   onDescriptionChange($("#description")[0], $("#group_info")[0]);

   $("#description").bind("keyup", function(){ onDescriptionChange($(this)[0], $("#group_info")[0]); } );

   HLPpopUpinitialize(null);

   if("All Channels" != $("#old_group_name").val())
   {
      MSCregister("CGedit" + $("#old_group_name").val(), $("#channels_left")[0], $("#channels_right")[0], 200, 420);
      MSCsetMode("CGedit" + $("#old_group_name").val(), "edit");
      CGEmode = "edit";

      $("div.excluded_member").bind("click", function(){ MSCtoggle("CGedit" + $("#old_group_name").val(), $(this)[0].id); } );
      $("div.included_member").bind("click", function(){ MSCtoggle("CGedit" + $("#old_group_name").val(), $(this)[0].id); } );

      $("div.excluded_member").bind("dblclick", function(){ MSCmove("CGedit" + $("#old_group_name").val(), {"id":$(this)[0].id}, "right"); } );
      $("div.included_member").bind("dblclick", function(){ MSCmove("CGedit" + $("#old_group_name").val(), {"id":$(this)[0].id}, "left"); } );

      filterTextChanged("right");
      filterTextChanged("left");

      $("#filter_left").bind("change", function(){ filterTextChanged("left"); } )
                       .bind("keyup", function(){ filterTextChanged("left"); } )
                       .bind("mouseover", function(){ MSCshowSearchTooltip($(this)[0]); } ) 
                       .bind("mouseout", function(){ MSChideSearchTooltip(); } )
                       .bind("mouseup", function(){ MSChideSearchTooltip(); } );

      $("#filter_right").bind("change", function(){ filterTextChanged("right"); } )
                        .bind("keyup", function(){ filterTextChanged("right"); } )
                        .bind("mouseover", function(){ MSCshowSearchTooltip($(this)[0]); } ) 
                        .bind("mouseout", function(){ MSChideSearchTooltip(); } )
                        .bind("mouseup", function(){ MSChideSearchTooltip(); } );

      $("#filter_clear_left").bind("click", function(){ $("#filter_left").val(""); filterTextChanged("left"); } );
      $("#filter_clear_right").bind("click", function(){ $("#filter_right").val(""); filterTextChanged("right"); } );

      $("#member_right_arrow").bind("click", function(){ moveSelected("left", "right", false); } )
                              .bind("mouseover", function(){ TOOLtooltipLink("Move selected.", null, $(this)[0]); } )
                              .bind("mouseout", function(){ TOOLtooltipClose(); } )
                              .bind("mouseup", function(){ TOOLtooltipClose(); } );

      $("#member_left_arrow").bind("click", function(){ moveSelected("right", "left", false); } )
                             .bind("mouseover", function(){ TOOLtooltipLink("Move selected.", null, $(this)[0]); } )
                             .bind("mouseout", function(){ TOOLtooltipClose(); } )
                             .bind("mouseup", function(){ TOOLtooltipClose(); } );

      $("#member_dbl_right_arrow").bind("click", function(){ moveSelected("left", "right", true); } )
                                  .bind("mouseover", function(){ TOOLtooltipLink("Move all.", null, $(this)[0]); } )
                                  .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                  .bind("mouseup", function(){ TOOLtooltipClose(); } );

      $("#member_dbl_left_arrow").bind("click", function(){ moveSelected("right", "left", true); } )
                                 .bind("mouseover", function(){ TOOLtooltipLink("Move all.", null, $(this)[0]); } )
                                 .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                 .bind("mouseup", function(){ TOOLtooltipClose(); } );
   }

   $(".helpIcon").bind("click", function(){ return false; } );

});
