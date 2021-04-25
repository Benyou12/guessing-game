package com.example.polychat.tutorial

import android.graphics.Color
import android.os.Bundle
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import com.demotxt.droidsrce.slide.SlideAdapter
import kotlinx.android.synthetic.main.activity_tutorial.*
import androidx.viewpager.widget.ViewPager.OnPageChangeListener
import com.example.polychat.R
import com.example.polychat.base.BaseActivity
import com.example.polychat.services.util.UIMode

class TutorialActivity : BaseActivity() {

    private var _adapter: SlideAdapter? = null
    private val progressBar: ArrayList<View> = arrayListOf()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.activity_tutorial)
        initView()
        initViewPager()
    }

    private fun initView() {
        progressBar.apply {
            add(block1)
            add(block2)
            add(block3)
            add(block4)
            add(block5)
            add(block6)
            add(block7)
        }
        button_got_it.apply {
            alpha = 0f
            isEnabled = false
        }
        button_got_it.setOnClickListener {
            navigateToHomeActivity()
        }
    }

    private fun initViewPager() {
        _adapter = SlideAdapter(this)
        viewpager_tutorial!!.adapter = _adapter
        viewpager_tutorial.addOnPageChangeListener(object : OnPageChangeListener {
            override fun onPageScrollStateChanged(state: Int) {
            }
            override fun onPageScrolled(position: Int,
                                        positionOffset: Float,
                                        positionOffsetPixels: Int) {
               updateView(position)
            }
            override fun onPageSelected(position: Int) {

            }
        })
    }

    private fun updateView(position: Int) {
        for(i in 0 until progressBar.size)
        {
            if(i <= position)
            {
                progressBar[i].setBackgroundColor(Color.parseColor("#0984e3"))
            }
            else
            {
                progressBar[i].setBackgroundColor(Color.parseColor("#40000000"))
            }
        }
        val isLastSlide = position == progressBar.size - 1
        if (isLastSlide)
        {
            button_got_it.apply {
                isEnabled = true
                alpha = 1f
            }
        }
    }
}
