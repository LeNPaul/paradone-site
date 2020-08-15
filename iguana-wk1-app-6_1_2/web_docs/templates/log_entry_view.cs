         <div id="entryView" <?cs if:!showEntryView ?>style="display:none"<?cs /if ?>>
            <table style="width:100%; table-layout:fixed">
               <tr><td style="padding:0px">
                  <table style="width:100%; border-collapse:collapse;"><tr>
                  
                     <!-- CHANNEL ICON/NAME TABLE CELL -->
                     <td align="left" style="padding:0px;">
                        <div id="entryDescription" style="height:36px; white-space:nowrap;"></div>
                     </td>
                  
                     <!-- BUTTONS TABLE CELL -->
                     <td>
                     
                        <!-- BEGIN DIV CONTAINER FOR BUTTONS -->
                        <div style="float: right; padding-top:1px;">

                        <?cs if:!minimalControls ?>
                           <div id="entryResubmit" style="padding:0 10px 0 0; float:left">
                              <img class="button" onClick="showResubmit()"
                                 <?cs call:imageTipCanAdmin(resubmit_tip,resubmit_tip_nonadmin) ?>
                                 src="<?cs call:imgSrcCanAdmin('images/log-browser-resubmit') ?>" />
                           </div>
                           <div id="entryRelated" style="padding:0 3px 0px 0px; float:left">
                              <img class="button" onClick="showRelated()"
                                 <?cs call:imageTip(related_tip) ?>
                                 src="<?cs var:skin('images/log-browser-related.gif') ?>" />
                           </div>
                           <div id="entryToMapper" style="padding:0 3px 0px 0px; float:left">
                              <img class="button" <?cs call:imageTip(to_mapper_tip) ?>
                                 src="<?cs var:skin('images/log-browser-to-mapper.gif') ?>" />
                           </div>
                           <div style="padding:0 10px 0 0; float:left">
                              <img class="button" onClick="showLink()"
                                 <?cs call:imageTip(link_tip) ?>
                                 src="<?cs var:skin('images/log-browser-link.gif') ?>" />
                           </div>
                           <div style="padding:0 10px 0 0; float:left">
                              <img class="button" onClick="downloadMessage()"
                                 <?cs call:imageTip(download_message_tip) ?>
                                 src="<?cs var:skin('images/log-browser-save-message.gif') ?>" />
                           </div>
                        <?cs /if ?>
                        
                        <?cs if:minimalControls ?>
                        <div style="float:left; color:#949494; padding: 7px 5px 0 0; font-weight:bold;">
                           View Mode:
                        </div>
                        <?cs /if ?>
                        
                        <div class="viewSelectDropdown" style="padding:0 3px 0 0px; float:left;">
                           <div class="dropdownButtonWrapper">
                              <img id="logViewSelectDropdownButton" class="ViewDropdownButton"
                                 src="<?cs var:skin('images/log_views/dropdown-text.gif') ?>"
                                 <?cs call:imageTip(view_tip) ?>
                                 onclick="javascript:toggleDropdown('logViewSelectDropdown', 'logViewSelectDropdownButton', CurrentViewMode);">
                              </img>
                           </div>
                           <div id="logViewSelectDropdown" style="display:none; <?cs if:minimalControls ?>right:43px;<?cs /if ?>"
                                onclick="javascript:hideDropdown('logViewSelectDropdown', 'logViewSelectDropdownButton', CurrentViewMode);">
                              <a class="logViewDropdownOption Hex" onclick="showHex();" <?cs call:imageTip(hex_tip) ?>>
                                 <img /><span>Hex-dump</span>
                              </a>
                              <a class="logViewDropdownOption Text" onclick="showText();" <?cs call:imageTip(text_tip) ?>>
                                 <img /><span>Plain-text</span>
                              </a>
                              <div class="logViewDropdownDivider messageOption" onclick="cancelBubble(event);"></div>
                              <a class="logViewDropdownOption SegmentMessage messageOption" onclick="showParsed('SegmentMessage', 'Preserve');" <?cs call:imageTip(parsedSgm_tip) ?>>
                                 <img /><span>Segment View</span>
                              </a>
                              <a class="logViewDropdownOption SegmentGrammar parseOption" onclick="showParsed('SegmentGrammar', 'Preserve');" <?cs call:imageTip(parsedSgG_tip) ?>>
                                 <img /><span>Segment Grammar View</span>
                              </a>
                              <div class="logViewDropdownDivider parseOption" onclick="cancelBubble(event);"></div>
                              <a class="logViewDropdownOption Table parseOption" onclick="showParsed('Table', 'Preserve');" <?cs call:imageTip(parsedTbl_tip) ?>>
                                 <img /><span>Table View (Graphical)</span>
                              </a>
                              <a class="logViewDropdownOption TableText parseOption" onclick="showParsed('TableText', 'Preserve');" <?cs call:imageTip(parsed_tip) ?>>
                                 <img /><span>Table View (Plain-text)</span>
                              </a>
                              <a class="logViewDropdownOption SQL parseOption" onclick="showParsed('SQL', 'Preserve');" <?cs call:imageTip(parsedSQL_tip) ?>>
                                 <img /><span>SQL View</span>
                              </a>
                           </div>
                        </div>
                        
                        <?cs if:!minimalControls ?><?cs set:wrapButtonLeftPadding = '10px' ?><?cs else ?><?cs set:wrapButtonLeftPadding = '0' ?><?cs /if ?>
                        <div id="entryWrapTextControl" style="padding:0 <?cs var:wrapButtonLeftPadding ?> 0 0; float:left;">
                           <img id="wrapText" class="button"
                              <?cs call:imageTip(wrap_tip) ?>
                              src="<?cs var:skin('images/log-browser-wrap.gif') ?>" />
                        </div>
                        <div id="entryWrapTextControlGrey" style="padding:0 <?cs var:wrapButtonLeftPadding ?> 0 0; float:left; display:none;">
                           <img id="wrapTextGrey" class="button"
                              <?cs call:imageTip(wrap_tip_grey) ?>
                              src="<?cs var:skin('images/log-browser-nowrap-grey.gif') ?>" />
                        </div>
                        
                        <?cs if:!minimalControls ?>
                           <div id="entryMarked" style="padding:0 10px 0 0; float:left">
                              <img id="entryMarkedImg" class="button" onClick="markEntry()"
                                 <?cs call:imageTip(mark_msg_tip) ?>
                                 src="<?cs var:skin('images/log-ack-yes') ?>" />
                           </div>
                        <?cs /if ?>
                        <!-- END DIV CONTAINER FOR BUTTONS -->
                        </div>
                     </td>
                     
                     <?cs if:!minimalControls ?>
                     <!-- DELETE BUTTON CELL -->
                     <td align="right" style="width:30px; padding: 0px;">
                        <div id="entryDelete" style="display:none; padding:3px 10px 0 0;">
                           <img class="button" onClick="deleteEntry()"
                              <?cs call:imageTipCanAdmin(delete_tip,delete_tip_nonadmin) ?>
                              src="<?cs call:imgSrcCanAdmin('images/log-browser-delete') ?>" />
                        </div>
                        <div id="entryUndelete" style="display:none; padding:3px 10px 0 0;">
                           <img class="button" onClick="undeleteEntry()"
                              <?cs call:imageTipCanAdmin(undelete_tip,undelete_tip_nonadmin) ?>
                              src="<?cs call:imgSrcCanAdmin('images/log-browser-undelete') ?>" />
                        </div>
                     </td>
                     
                     <!-- NAVIGATION TABLE CELL (Next/Prev/Pop-out/Close) -->
                     <td align="right" style="width:161px; padding: 0px; border-left: 1px solid #d0d1d1;">
                        <table style="border-collapse:collapse; margin-top:1px;">
                           <tr>
                              <td style="padding-right:3px; background:url('/images/log_buttons_bg_left.gif'); background-repeat:no-repeat; background-position:left;">
                                 <div style="white-space:nowrap; margin-top:0px; padding-top:1px;">
                                    <img class="nav-button" id="entryPrev" />
                                    <img class="nav-button" id="entryNext" />
                                    <!-- 
                                       <img class="nav-button" id="entryPopout" src="/<?cs var:skin('images/msg-pop-out.gif') ?>"
                                       onmouseover="javascript:this.src='/<?cs var:skin('images/msg-pop-out-hover.gif') ?>'; TOOLtooltipLink('View this entry in a new window.', undefined, this, { Left: -12 });"
                                       onmouseout ="javascript:this.src='/<?cs var:skin('images/msg-pop-out.gif') ?>';       TOOLtooltipClose();"
                                       onmousedown="javascript:this.src='/<?cs var:skin('images/msg-pop-out-active.gif') ?>';"
                                       onmouseup  ="javascript:this.src='/<?cs var:skin('images/msg-pop-out-hover.gif') ?>';"
                                       onclick    ="javascript:LOGpopoutCurrentEntry();" /> -->
                                 </div>
                              </td>
                              <td style="padding-left:3px; background:url('/images/log_buttons_bg_right.gif'); background-repeat:no-repeat; background-position:right;">
                                 <div style="white-space:nowrap; margin:-1px;">
                                    <a class="button" onClick="showLog()"><span>Close</span></a>
                                 </div>
                              </td>
                           </tr>
                        </table>
                     </td>
                     <?cs /if ?>
                     
                  </tr></table>
               </td></tr>
               <tr><td>
                  <div id="PleaseWaitDiv" class="entryViewPleaseWaitBar" style="position:absolute; visibility:hidden;">
                     <-- filled by js-->
                  </div>
                  <div id="entryArea" style="height:100%;">
                     <div class="entryViewPleaseWaitBar">Processing...</div>
                  </div>
                  <div id="reparseWarning" style="display:none;">
                  </div>
               </td></tr>
            </table>
         </div>
         
