<p>Use the <strong>Text Query</strong> field to narrow down your search with specific text matching. You can include multiple words, exclude words, or use regular expressions to filter search results.</p>

<p>For example, this text query combines Boolean and regex syntax to perform a very specific search:</p>

<p style=&quot;white-space:nowrap;&quot;><strong>Facility NOT /Facility[2-4]/ /(In|Out)bound/</strong></p>

<p>This search will find all channels with <strong>Facility</strong> in their name, but will skip those named <strong>Facility2</strong>, <strong>Facility3</strong> and <strong>Facility4</strong>.  In addition, this search will only find channels with the words <strong>Inbound</strong> or <strong>Outbound</strong> included in their name.</p>