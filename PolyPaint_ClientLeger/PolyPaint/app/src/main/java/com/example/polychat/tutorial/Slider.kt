package com.demotxt.droidsrce.slide

import android.content.Context
import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.LinearLayout
import androidx.viewpager.widget.PagerAdapter
import com.example.polychat.R
import com.example.polychat.tutorial.*
import kotlinx.android.synthetic.main.layout_tutorial_slide.view.*

class SlideAdapter(internal var context: Context) : PagerAdapter() {

    // todo add an image of each features
    private val featuresSnapshot = intArrayOf(
            R.drawable.img1,
            R.drawable.img2,
            R.drawable.img3,
            R.drawable.img4,
            R.drawable.img5,
            R.drawable.img6,
            R.drawable.img7
    )

    private val featuresTitle = arrayOf(
            title1.getValue(),
            title2.getValue(),
            title3.getValue(),
            title4.getValue(),
            title5.getValue(),
            title6.getValue(),
            title7.getValue()
    )

    private val featuresDescriptions = arrayOf(
            msg1.getValue(),
            msg2.getValue(),
            msg3.getValue(),
            msg4.getValue(),
            msg5.getValue(),
            msg6.getValue(),
            msg7.getValue()
    )

    private val backgroundColors = intArrayOf(
            Color.rgb(55, 55, 55),
            Color.rgb(87, 88, 187),
            Color.rgb(0, 0, 0),
            Color.rgb(234, 32, 39),
            Color.rgb(110, 49, 89),
            Color.rgb(238, 90, 36),
            Color.rgb(18, 137, 167)
    )

    override fun getCount(): Int {
        return featuresSnapshot.size
    }

    override fun isViewFromObject(view: View, obj: Any): Boolean {
        return view === obj as LinearLayout
    }

    override fun instantiateItem(container: ViewGroup, position: Int): View {
        val inflater =  context.getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val view = inflater.inflate(R.layout.layout_tutorial_slide, container, false)
        view.linear_layout_slide.setBackgroundColor(backgroundColors[position])
        view.image_view_tutorial_slide.setImageResource(featuresSnapshot[position])
        view.texte_view_title_tutorial_slide.text = featuresTitle[position]
        view.texte_view_desc_tutorial_slide.text = featuresDescriptions[position]
        container.addView(view)
        return view
    }

    override fun destroyItem(container: ViewGroup, position: Int, obj: Any) {
        container.removeView(obj as LinearLayout)
    }
}